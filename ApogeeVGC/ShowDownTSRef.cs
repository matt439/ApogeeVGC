//runStatusImmunity(type: string, message ?: string) {
//    if (this.fainted) return false;
//    if (!type) return true;

//    if (!this.battle.dex.getImmunity(type, this))
//    {
//        this.battle.debug('natural status immunity');
//        if (message)
//        {
//            this.battle.add('-immune', this);
//        }
//        return false;
//    }
//    const immunity = this.battle.runEvent('Immunity', this, null, null, type);
//    if (!immunity)
//    {
//        this.battle.debug('artificial status immunity');
//        if (message && immunity !== null)
//        {
//            this.battle.add('-immune', this);
//        }
//        return false;
//    }
//    return true;
//}