#pragma once

struct GameTimerSettings
{
    bool dc_timer = false;
    bool dc_timer_bank = false;
    int starting = 0;
    int grace = 0;
    int add_per_turn = 0;
    int max_per_turn = 0;
    int max_first_turn = 0;
    bool timeout_auto_choose = false;
    bool accelerate = false;
};
