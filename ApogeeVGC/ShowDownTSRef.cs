//makeRequest(type ?: RequestState) {
//    if (type)
//    {
//        this.requestState = type;
//        for (const side of this.sides) {
//            side.clearChoice();
//        }
//    }
//    else
//    {
//        type = this.requestState;
//    }

//    for (const side of this.sides) {
//        side.activeRequest = null;
//    }

//    if (type === 'teampreview')
//    {
//        // `pickedTeamSize = 6` means the format wants the user to select
//        // the entire team order, unlike `pickedTeamSize = undefined` which
//        // will only ask the user to select their lead(s).
//        const pickedTeamSize = this.ruleTable.pickedTeamSize;
//        this.add(`teampreview${ pickedTeamSize ? `|${ pickedTeamSize}` : ''}`);
//    }

//    const requests = this.getRequests(type);
//    for (let i = 0; i < this.sides.length; i++)
//    {
//        this.sides[i].activeRequest = requests[i];
//    }
//    this.sentRequests = false;

//    if (this.sides.every(side => side.isChoiceDone()))
//    {
//        throw new Error(`Choices are done immediately after a request`);
//    }
//}