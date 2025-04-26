datablock TSShapeConstructor(WrathfulDts)
{
    baseShape  = "./wrathful.dts";
    sequence0  = "./root.dsq root";

    sequence1  = "./run.dsq run";
    sequence2  = "./root.dsq walk";
    sequence3  = "./back.dsq back";
    sequence4  = "./side.dsq side";

    sequence5  = "./crouch.dsq crouch";
    sequence6  = "./crouchRun.dsq crouchRun";
    sequence7  = "./crouchBack.dsq crouchBack";
    sequence8  = "./crouchSide.dsq crouchSide";

    sequence9  = "./look.dsq look";
    sequence10 = "./headside.dsq headside";
    sequence11 = "./root.dsq headUp";

    sequence12 = "./jump.dsq jump";
    sequence13 = "./standJump.dsq standjump";
    sequence14 = "./fall.dsq fall";
    sequence15 = "./root.dsq land";

    sequence16 = "./armAttack.dsq armAttack";
    sequence17 = "./armReadyLeft.dsq armReadyLeft";
    sequence18 = "./armReadyRight.dsq armReadyRight";
    sequence19 = "./armReadyBoth.dsq armReadyBoth";
    sequence20 = "./spearReady.dsq spearready";  
    sequence21 = "./spearThrow.dsq spearThrow";

    sequence22 = "./talk.dsq talk";  

    sequence23 = "./death1.dsq death1"; 

    sequence24 = "./shiftUp.dsq shiftUp";
    sequence25 = "./shiftDown.dsq shiftDown";
    sequence26 = "./shiftAway.dsq shiftAway";
    sequence27 = "./shiftTo.dsq shiftTo";
    sequence28 = "./shiftLeft.dsq shiftLeft";
    sequence29 = "./shiftRight.dsq shiftRight";
    sequence30 = "./rotCW.dsq rotCW";
    sequence31 = "./rotCCW.dsq rotCCW";

    sequence32 = "./undo.dsq undo";
    sequence33 = "./plant.dsq plant";

    sequence34 = "./sit.dsq sit";

    sequence35 = "./wrench.dsq wrench";

    //sequence36 = "./activate.dsq activate";
    //sequence37 = "./activate2.dsq activate2";

    sequence38 = "./leftRecoil.dsq leftrecoil";
};    

datablock PlayerData(PlayerWrathfulArmor : PlayerStandardArmor)
{
   shapeFile = "./wrathful.dts";

	uiName = "The Wrathful";
    canjet = false;
   //cameraVerticalOffset = 0.8;
};