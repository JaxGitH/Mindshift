using UnityEngine;
// This script handles the Scriptable Object that will hold the sound effects and music to draw from.
// To add new sound effects to the array, add the name to the appropiate enum list.
// Last updated: 2/19/25
public enum eEffects {
    //<General UI sounds>
    //General sound effects
    click, confirm, back, pause,
    
    //<Character specific>
    //General character movement
    walkDefault, walkWood, walkMetal, walkIce, walkGlass, climb, ledgeJump,
    //Death
    deathFire, deathWater, deathPit, deathSpikes, deathZap,
    
    //<Interactable objects>
    //Heavy Cube
    cubeDrop,
    //Wooden Plank
    woodClank, woodFloat,
    //Teleporter
    teleporterHum, teleporterIn, teleporterOut,
    //Balloon
    balloonAttatched, balloonDettached, balloonPop,
    //Randobox
    randoBoxIdle, randoBoxSlot, randoBoxBounce, randoBoxPoof,
    //Bounce Pad
    bouncePadBoing, bouncePadRetract,
    //See saw
    seeSawHigh, seeSawLow,
    //Magnet
    magnetHum, magnetAttached,
    //Fan
    fanOn, fanOff,
    
    //<Hazards>
    //Electricity
    spark,

    //<Environment>
    //Moving Platform
    platformHum, platformStop,
    //Button
    buttonOn, buttonOff,
    //Lever
    leverFlick,
    //Pressure Plates
    plateOn, plateOff,

    //<Ambiance>
    //Sterile Laboratory
    sterileAmba1, sterileAmba2, sterileAmba3,
    //Cryostasis Laboratory
    cryoAmba1, cryoAmba2, cryoAmba3, cryoAmba4,
    //Radioactive Quarantine Laboratory
    radioactiveAmba1, radioactiveAmba2, radioactiveAmba3,
}
public enum eSongs { 
    //Front End
    mainMenu, options, 
    //Levels
    tutorial, sterileMusic, cryoMusic, radioactiveMusic,
    //Misc.
    extras, credits,
    }

[CreateAssetMenu(fileName = "New SoundLibrary", menuName = "Create SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    [NamedArray(typeof(eEffects))] public AudioClip[] effects;
    [NamedArray(typeof(eSongs))] public AudioClip[] songs;
}
