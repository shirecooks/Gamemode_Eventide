// Generic weapon trails:
// - Axe and katana use the same generic straight trail shape.
// - Axe's trail can be scaled up a bit to make it chunkier.
// - Machete can use the ragged trail texture from the ragged claw trail (use mesh skinning).

%root = filePath($Con::File);

addExtraResource(%root @ "/models/trails/base.trail.png");
addExtraResource(%root @ "/models/trails/ragged.trail.png");

// Killer claw trails:
// Both a ragged and straight claw trail shape (use mesh skinning).

addExtraResource(%root @ "/models/trails/baseClaw.trail.png");
addExtraResource(%root @ "/models/trails/raggedClaw.trail.png");

// Killer other trails:

addExtraResource(%root @ "/models/trails/magic.trail.png");
addExtraResource(%root @ "/models/trails/glitch.trail.png");

datablock StaticShapeData(KillerTrailShape)
{
	shapeFile = "./models/trails/skinnableTrail.dts";
};