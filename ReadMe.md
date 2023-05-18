# Tutorial
- There is no tutorial implemented for the project. Please take a look at the video sent via email for more details how to play the game
- In general, objects can be dragged "upward" until it detachs from the ground or the object it's connected to. Once it's detached, it can be dropped to another location
- Many object can be rotate, slide, etc... by dragging it
- Tap on objects to select them. After selected, they can be deleted.

# Code overview
- All the code are written by me
- Some assemblies are brought from my other projects:
	- Geometry: curves, meshes, shapes, etc...
	- AssetPipeline: to batch-process assets
	- ClippingPortal: to render object behind a portal, without using render-to-texture
- Other assemblies:
	- GameWorlds: written from scratch for this project. Base architecture for the game.
	- MoveConstraints: allow objects to be moved in certain restricted constraint, such as slider, hinge or ball joints. This was implemented from my other project and further improved in this project.
	- Platforms: allow objects to be placed on different type of surfaces, such as Grid, IcoSphere or Custom
	- Interactions: object selection
- Some codes were converted from DOTS
	- You would find a lot of struct, generic and native container. The reason was to make it compatible with Bursts. I tried to convert as much as I can.
# Features
- I try to cover all requirements from the Test
- I choose some "innovative" features instead of standard ones, for example:
	- You can build sophisticated mechanism out of the basic blocks, similar to Lego Technic
	- Undo is possible even after Loading
	- I made dedicated Save and Load buttons so that serialization can be tested more easiliy without restarting the game
	- The Clipping Portal feature should work with the rest of other features, but I added it in the last minute so I could not add more content for it.