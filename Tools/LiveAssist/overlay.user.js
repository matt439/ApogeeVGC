// ==UserScript==
// @name         Apogee VGC Live Assist
// @namespace    apogee-vgc
// @version      1.0
// @description  Bridges Pokemon Showdown battles to the Apogee VGC AI server
// @match        https://play.pokemonshowdown.com/*
// @match        https://replay.pokemonshowdown.com/*
// @grant        none
// ==/UserScript==

(function () {
    'use strict';

    const WS_URL = 'ws://localhost:9876';
    const RECONNECT_DELAY = 3000;

    let ws = null;
    let overlay = null;
    let connected = false;

    // ── WebSocket connection ────────────────────────────────────────────────

    function connect() {
        if (ws && ws.readyState <= 1) return;

        ws = new WebSocket(WS_URL);

        ws.onopen = () => {
            connected = true;
            updateStatus('Connected', 'lime');
            console.log('[Apogee] Connected to server');
        };

        ws.onclose = () => {
            connected = false;
            updateStatus('Disconnected', '#ff4444');
            console.log('[Apogee] Disconnected, reconnecting...');
            setTimeout(connect, RECONNECT_DELAY);
        };

        ws.onerror = () => {
            connected = false;
            updateStatus('Error', '#ff4444');
        };

        ws.onmessage = (event) => {
            try {
                const msg = JSON.parse(event.data);
                handleServerMessage(msg);
            } catch (e) {
                console.error('[Apogee] Failed to parse server message:', e);
            }
        };
    }

    function send(data) {
        if (ws && ws.readyState === WebSocket.OPEN) {
            ws.send(JSON.stringify(data));
        }
    }

    // ── Hook into Showdown ──────────────────────────────────────────────────

    function hookShowdown() {
        // Wait for PS to be available
        if (typeof PS === 'undefined') {
            setTimeout(hookShowdown, 500);
            return;
        }

        // Hook PS.receive() to intercept all incoming messages
        const origReceive = PS.receive.bind(PS);
        PS.receive = function (msg) {
            // Forward battle messages to our server
            if (msg.startsWith('>battle-')) {
                send({ type: 'battle', data: msg });
            }
            origReceive(msg);
        };

        // Also watch for request updates on battle rooms
        // PS.rooms is a Map of room ID -> Room object
        const checkRooms = setInterval(() => {
            if (!PS.rooms) return;

            for (const [id, room] of PS.rooms) {
                if (!id.startsWith('battle-')) continue;
                if (room._apogeeHooked) continue;

                // Hook the room's receiveLine to catch |request| messages
                room._apogeeHooked = true;
                const origReceiveLine = room.receiveLine.bind(room);
                room.receiveLine = function (args) {
                    origReceiveLine(args);

                    // After Showdown processes the line, check for request updates
                    if (args[0] === 'request' && room.request) {
                        send({
                            type: 'request',
                            room: id,
                            data: room.request,
                        });
                    }
                };

                console.log(`[Apogee] Hooked battle room: ${id}`);
            }
        }, 1000);

        console.log('[Apogee] Hooked PS.receive()');
    }

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
                <button id="apogee-minimize">−</button>
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
            btn.textContent = content.classList.contains('hidden') ? '+' : '−';
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

        // Value bar
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
                // Clean up action name for display
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

        // Sort by bring score descending
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

    function init() {
        createOverlay();
        connect();
        hookShowdown();
    }

    // Wait for page load
    if (document.readyState === 'complete') {
        init();
    } else {
        window.addEventListener('load', init);
    }
})();
