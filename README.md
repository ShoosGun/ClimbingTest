# Collider Visualizer

# ![Collider Visualizer](pictures/colliderVisualizer.png)

This is a mod for Outer Wilds which allows the player to visualize diferent kinds of colliders (and Shapes) that exist in the game.

Settings Options:

* **Draw?** - Enables or dissables the rendering of colliders and shapes.
* **toggleDrawKB** - The keyboard action that toggles the **Draw?** option, it by default is set to *f9*, but you can change to whichever key or key combination by following the unity scheme [here](https://docs.unity3d.com/ScriptReference/Event.KeyboardEvent.html).

* **Radius of search for colliders** - The radius (in ingame meters) of where the colliders are going to be searched on, lower values might increase performance,, this option doens't affect shapes.

* **Amount of colliders to draw** - The amount of colliders that are going to be draw from the search, lower values might increase performance, this option doens't affect shapes.

* **checkFrequency** - The amount of times per second that the search will happen, lower values might increase performance, this option doens't affect shapes.

* **Draw Bounding Boxes?** - Enables or dissables the rendering of colliders bounding boxes.

* **Draw Triggers?** - Enables or dissables the rendering of trigger colliders.

* **Draw Physical Colliders?** - Enables or dissables the rendering of non trigger colliders.

* **Draw Shape Bounds?** - Enables or dissables the rendering the bounding sphere of shapes.

* **Draw Shape Detectors?** - Enables or dissables the rendering the shape detectors.

* **shapeLayers** - This is an option to select which layers from the shape volumes should be drawn, you can select which layers by writing its number with separation characters ('-', ' ', ':', ',', '.') like in *1 3 4* . There are 4 layers in total, which have different porpuses in game:
    * 1 - General Volumes, ie, Ghost Matter Volumes, Geiser Volumes, Player Attach Points;
    * 2 - Planet Volumes;
    * 3 - ?;
    * 4 - Flashlight Volumes;

* **Draw Shape Volumes?** - Enables or dissables the rendering the shape volumes.


