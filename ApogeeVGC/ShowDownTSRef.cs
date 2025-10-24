///**
//     * Generally called at the beginning of a turn, to go through the
//     * turn one action at a time.
//     *
//     * If there is a mid-turn decision (like U-Turn), this will return
//     * and be called again later to resume the turn.
//     */
//turnLoop() {
//    this.add('');
//    this.add('t:', Math.floor(Date.now() / 1000));
//    if (this.requestState) this.requestState = '';

//    if (!this.midTurn)
//    {
//        this.queue.insertChoice({ choice: 'beforeTurn' });
//        this.queue.addChoice({ choice: 'residual' });
//        this.midTurn = true;
//    }

//    let action;
//    while ((action = this.queue.shift()))
//    {
//        this.runAction(action);
//        if (this.requestState || this.ended) return;
//    }

//    this.endTurn();
//    this.midTurn = false;
//    this.queue.clear();
//}