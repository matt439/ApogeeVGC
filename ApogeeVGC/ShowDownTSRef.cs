//win(side ?: SideID | '' | Side | null) {
//    if (this.ended) return false;
//    if (side && typeof side === 'string')
//    {
//        side = this.getSide(side);
//    }
//    else if (!side || !this.sides.includes(side))
//    {
//        side = null;
//    }
//    this.winner = side ? side.name : '';

//    this.add('');
//    if (side?.allySide)
//    {
//        this.add('win', side.name + ' & ' + side.allySide.name);
//    }
//    else if (side)
//    {
//        this.add('win', side.name);
//    }
//    else
//    {
//        this.add('tie');
//    }
//    this.ended = true;
//    this.requestState = '';
//    for (const s of this.sides) {
//        if (s) s.activeRequest = null;
//    }
//    return true;
//}