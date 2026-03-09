// ==UserScript==
// @name         Apogee VGC Live Assist
// @namespace    apogee-vgc
// @version      1.1
// @description  Bridges Pokemon Showdown battles to the Apogee VGC AI server
// @match        https://play.pokemonshowdown.com/*
// @match        http://play.pokemonshowdown.com/*
// @match        https://replay.pokemonshowdown.com/*
// @match        http://replay.pokemonshowdown.com/*
// @match        http://psim.us/*
// @match        https://psim.us/*
// @grant        none
// @run-at       document-start
// ==/UserScript==

(function () {
    'use strict';

    const WS_URL = 'ws://localhost:9876';
    const RECONNECT_DELAY = 3000;

    let apogeeWs = null;
    let overlay = null;
    let connected = false;

    // ── Apogee WebSocket connection ─────────────────────────────────────────

    function connectApogee() {
        if (apogeeWs && apogeeWs.readyState <= 1) return;

        apogeeWs = new WebSocket(WS_URL);

        apogeeWs.onopen = () => {
            connected = true;
            updateStatus('Connected', 'lime');
            console.log('[Apogee] Connected to server');
        };

        apogeeWs.onclose = () => {
            connected = false;
            updateStatus('Disconnected', '#ff4444');
            console.log('[Apogee] Disconnected, reconnecting...');
            setTimeout(connectApogee, RECONNECT_DELAY);
        };

        apogeeWs.onerror = () => {
            connected = false;
            updateStatus('Error', '#ff4444');
        };

        apogeeWs.onmessage = (event) => {
            try {
                const msg = JSON.parse(event.data);
                handleServerMessage(msg);
            } catch (e) {
                console.error('[Apogee] Failed to parse server message:', e);
            }
        };
    }

    function send(data) {
        if (apogeeWs && apogeeWs.readyState === WebSocket.OPEN) {
            apogeeWs.send(JSON.stringify(data));
        }
    }

    // ── Intercept Showdown's WebSocket via constructor hook ──────────────────
    //
    // We patch the WebSocket constructor BEFORE Showdown's JS loads (run-at: document-start).
    // Every WebSocket created by Showdown goes through our wrapper. We attach an
    // onmessage listener to forward battle messages to our server.
    //
    // This avoids needing access to the PS global (which is module-scoped).

    const OrigWebSocket = window.WebSocket;

    window.WebSocket = function (...args) {
        const socket = new OrigWebSocket(...args);
        const url = args[0] || '';

        // Only intercept Showdown's connection (sim.smogon.com or similar)
        if (url.includes('sim') || url.includes('showdown') || url.includes('psim')) {
            console.log('[Apogee] Intercepted Showdown WebSocket:', url);

            socket.addEventListener('message', (event) => {
                const raw = '' + event.data;

                // SockJS wraps messages in frames:
                //   'o' = open, 'h' = heartbeat, 'c[...]' = close
                //   'a["msg1","msg2"]' = array of messages
                // Unwrap SockJS framing to get the actual protocol messages.
                let messages = [];
                if (raw.startsWith('a[')) {
                    try {
                        messages = JSON.parse(raw.slice(1));
                    } catch (e) {
                        return;
                    }
                } else if (raw.startsWith('>') || raw.startsWith('|')) {
                    // Raw protocol (non-SockJS connection)
                    messages = [raw];
                } else {
                    // SockJS control frames (o, h, c) — ignore
                    return;
                }

                for (const msg of messages) {
                    // Battle protocol messages start with >battle-
                    if (msg.startsWith('>battle-')) {
                        console.log('[Apogee] Battle message, length:', msg.length);
                        send({ type: 'battle', data: msg });
                    }

                    // |request| lines contain own-team data + legal actions
                    if (msg.includes('|request|')) {
                        const lines = msg.split('\n');
                        for (const line of lines) {
                            if (line.startsWith('|request|')) {
                                const jsonStr = line.slice('|request|'.length);
                                if (jsonStr) {
                                    try {
                                        const request = JSON.parse(jsonStr);
                                        let room = '';
                                        if (msg.startsWith('>')) {
                                            room = msg.slice(1, msg.indexOf('\n'));
                                        }
                                        console.log('[Apogee] Request intercepted for', room);
                                        send({
                                            type: 'request',
                                            room: room,
                                            data: request,
                                        });
                                    } catch (e) {
                                        console.error('[Apogee] Failed to parse request:', e);
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        return socket;
    };

    // Preserve prototype chain so instanceof checks still work
    window.WebSocket.prototype = OrigWebSocket.prototype;
    window.WebSocket.CONNECTING = OrigWebSocket.CONNECTING;
    window.WebSocket.OPEN = OrigWebSocket.OPEN;
    window.WebSocket.CLOSING = OrigWebSocket.CLOSING;
    window.WebSocket.CLOSED = OrigWebSocket.CLOSED;

    console.log('[Apogee] WebSocket constructor patched');

    // ── Handle server messages ──────────────────────────────────────────────

    function handleServerMessage(msg) {
        switch (msg.type) {
            case 'recommendation':
                showRecommendation(msg);
                break;
            case 'team_preview':
                showTeamPreview(msg);
                break;
        }
    }

    // ── Overlay UI ──────────────────────────────────────────────────────────

    function createOverlay() {
        overlay = document.createElement('div');
        overlay.id = 'apogee-overlay';
        overlay.innerHTML = `
            <div id="apogee-header">
                <span id="apogee-title">Apogee VGC</span>
                <span id="apogee-status">Connecting...</span>
                <button id="apogee-minimize">\u2212</button>
            </div>
            <div id="apogee-content">
                <div id="apogee-value"></div>
                <div id="apogee-slots"></div>
            </div>
        `;

        const style = document.createElement('style');
        style.textContent = `
            #apogee-overlay {
                position: fixed;
                bottom: 10px;
                right: 10px;
                width: 280px;
                background: rgba(20, 20, 30, 0.95);
                border: 1px solid #444;
                border-radius: 8px;
                color: #eee;
                font-family: 'Segoe UI', Arial, sans-serif;
                font-size: 13px;
                z-index: 99999;
                box-shadow: 0 4px 16px rgba(0,0,0,0.5);
                overflow: hidden;
            }
            #apogee-header {
                display: flex;
                align-items: center;
                padding: 6px 10px;
                background: rgba(40, 40, 60, 0.9);
                cursor: move;
                user-select: none;
            }
            #apogee-title {
                font-weight: bold;
                flex: 1;
                font-size: 14px;
            }
            #apogee-status {
                font-size: 11px;
                margin-right: 8px;
            }
            #apogee-minimize {
                background: none;
                border: none;
                color: #aaa;
                font-size: 18px;
                cursor: pointer;
                padding: 0 4px;
                line-height: 1;
            }
            #apogee-minimize:hover { color: #fff; }
            #apogee-content {
                padding: 8px 10px;
            }
            #apogee-content.hidden {
                display: none;
            }
            #apogee-value {
                margin-bottom: 8px;
            }
            .apogee-value-bar {
                height: 20px;
                border-radius: 3px;
                background: #333;
                overflow: hidden;
                position: relative;
            }
            .apogee-value-fill {
                height: 100%;
                border-radius: 3px;
                transition: width 0.3s ease;
            }
            .apogee-value-label {
                position: absolute;
                top: 0;
                left: 0;
                right: 0;
                text-align: center;
                line-height: 20px;
                font-weight: bold;
                font-size: 12px;
                text-shadow: 0 1px 2px rgba(0,0,0,0.8);
            }
            .apogee-slot {
                margin-bottom: 6px;
            }
            .apogee-slot-title {
                font-weight: bold;
                font-size: 12px;
                color: #88ccff;
                margin-bottom: 2px;
            }
            .apogee-action {
                display: flex;
                justify-content: space-between;
                padding: 2px 4px;
                border-radius: 3px;
            }
            .apogee-action.top { background: rgba(76, 175, 80, 0.3); }
            .apogee-action.mid { background: rgba(255, 193, 7, 0.15); }
            .apogee-action.low { background: rgba(255, 255, 255, 0.05); }
            .apogee-prob {
                font-weight: bold;
                min-width: 45px;
                text-align: right;
            }
            .apogee-preview-item {
                display: flex;
                justify-content: space-between;
                padding: 3px 4px;
                border-radius: 3px;
                margin-bottom: 2px;
            }
            .apogee-bring { color: #4caf50; font-weight: bold; }
            .apogee-lead { color: #2196f3; }
        `;

        document.head.appendChild(style);
        document.body.appendChild(overlay);

        // Minimize toggle
        document.getElementById('apogee-minimize').addEventListener('click', () => {
            const content = document.getElementById('apogee-content');
            const btn = document.getElementById('apogee-minimize');
            content.classList.toggle('hidden');
            btn.textContent = content.classList.contains('hidden') ? '+' : '\u2212';
        });

        // Dragging
        makeDraggable(overlay, document.getElementById('apogee-header'));
    }

    function makeDraggable(el, handle) {
        let isDragging = false;
        let startX, startY, origX, origY;

        handle.addEventListener('mousedown', (e) => {
            isDragging = true;
            startX = e.clientX;
            startY = e.clientY;
            const rect = el.getBoundingClientRect();
            origX = rect.left;
            origY = rect.top;
            e.preventDefault();
        });

        document.addEventListener('mousemove', (e) => {
            if (!isDragging) return;
            el.style.left = (origX + e.clientX - startX) + 'px';
            el.style.top = (origY + e.clientY - startY) + 'px';
            el.style.right = 'auto';
            el.style.bottom = 'auto';
        });

        document.addEventListener('mouseup', () => {
            isDragging = false;
        });
    }

    function updateStatus(text, color) {
        const el = document.getElementById('apogee-status');
        if (el) {
            el.textContent = text;
            el.style.color = color;
        }
    }

    function showRecommendation(msg) {
        const content = document.getElementById('apogee-content');
        if (!content) return;

        const valuePct = (msg.value * 100).toFixed(1);
        const valueColor = msg.value >= 0.55 ? '#4caf50' : msg.value >= 0.45 ? '#ffc107' : '#f44336';

        let html = `
            <div id="apogee-value">
                <div class="apogee-value-bar">
                    <div class="apogee-value-fill" style="width:${valuePct}%;background:${valueColor}"></div>
                    <div class="apogee-value-label">${valuePct}% Win</div>
                </div>
            </div>
            <div id="apogee-slots">
        `;

        for (const [label, actions] of [['Slot A', msg.slotA], ['Slot B', msg.slotB]]) {
            html += `<div class="apogee-slot"><div class="apogee-slot-title">${label}</div>`;
            for (const a of (actions || [])) {
                const pct = (a.prob * 100).toFixed(1);
                const cls = a.prob >= 0.5 ? 'top' : a.prob >= 0.2 ? 'mid' : 'low';
                const name = a.action.replace('move:', '').replace('switch:', 'Switch ');
                html += `<div class="apogee-action ${cls}">
                    <span>${name}</span>
                    <span class="apogee-prob">${pct}%</span>
                </div>`;
            }
            html += '</div>';
        }

        html += '</div>';
        content.innerHTML = html;
    }

    function showTeamPreview(msg) {
        const content = document.getElementById('apogee-content');
        if (!content) return;

        let html = '<div id="apogee-slots"><div class="apogee-slot-title">Team Preview</div>';

        const pokemon = (msg.pokemon || []).sort((a, b) => b.bringScore - a.bringScore);

        for (const p of pokemon) {
            const bringPct = (p.bringScore * 100).toFixed(0);
            const leadPct = (p.leadScore * 100).toFixed(0);
            html += `<div class="apogee-preview-item">
                <span>${p.species}</span>
                <span>
                    <span class="apogee-bring">B:${bringPct}%</span>
                    <span class="apogee-lead">L:${leadPct}%</span>
                </span>
            </div>`;
        }

        html += '</div>';
        content.innerHTML = html;
    }

    // ── Initialize ──────────────────────────────────────────────────────────

    // WebSocket hook is already active (runs at document-start).
    // Overlay + server connection start when DOM is ready.
    function initUI() {
        console.log('[Apogee] Initializing UI on:', window.location.href);
        createOverlay();
        connectApogee();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initUI);
    } else {
        initUI();
    }
})();
