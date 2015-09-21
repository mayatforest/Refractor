# Refractor
Fork of Refractor by hughdoar@hotmail.com 

Diagrams with Reflector and the Graph Plug-in
http://www.codeproject.com/Articles/29724/Diagrams-with-Reflector-and-the-Graph-Plug-in-Part

Introduction
In this article, we're going to try to generate useful diagrams of assemblies. I'm interested in assemblies primarily during the period of their development, but trying to understand an already completed assembly is a similar task.

The previous article (Diagrams with Reflector and the Graph Plugin (Part 1)) used Reflector as a host, hence the name of the article, but the new plug-ins use a new host called Refractor. This is all included in the download.

Background
--------
Previously, we dealt with graphically showing which methods call which other methods, within a class. Classes come in all shapes and sizes, but the ones we're interested in diagramming are generally the ones that are difficult to understand. Typically, these include WinForms classes, where the linkage generated by events calling common methods rapidly becomes confusing. This was implemented via a plug-in to Reflector, and worked quite well.

This has already proved useful to me in my everyday job in several different ways:

Whilst building a class, it's useful to keep track of the method call graph. Sometimes, I do this by hand, but having it readily available means I refer to it more often.
While trying to understand someone else's class. Generally, I have the full source code for this, but it's probably been written by more than one person, and over a period of time, that leaves it difficult to understand.
Whilst reviewing code, I can more easily check for oddities which can suggest a design issue.
I wanted to see if there were other useful diagrams out there, in "diagram-space". Quite quickly in fact, I needed diagrams of assembly dependencies whilst fixing a large build process, and I put together a similar plug-in to show this, for all assemblies within a project.

However, I had some issues with the host plug-in architecture.

It was more difficult coding within the plug-in framework, in particular against an obfuscated host exe.
The diagrams could take a long time to generate. There's no threading support in the host, so if I wanted threading, I'd have to bolt it into the plug-in myself, which seemed a little daunting at the time.
I was very tired of hitting refresh and re-selecting my item for diagramming. It would be nice if the host could check for file updates, and re-load. Most people use Reflector to examine assemblies for which they don't have the source, so the assembly won't be changing. When you're developing an assembly, it changes all the time. This points to a different style of user for the host.
I'm generally not interested in modules or resources, and the host tree was more confusing due to the inclusion of this, sometimes vital, information.
I wasn't interested in the disassembly, which is the primary function of the host. Saying that, it's nice to have sometimes, and I've bolted in a disassembly viewer to the current host. No decompiler though!
Lutz wasn't happy with me bundling Reflector with the plug-in, which is fair enough, but leaves me unable to produce a full working download.
Hence the new host.

Diagrams
--------
Following on from the ClassMethods diagram of the first article, we have three more diagrams:

- AssemblyNamespaces: This has not been so useful for me, as most assemblies I deal with contain a single namespace, although it's good for seeing the exceptions to the case.
NamespaceClasses: This has proved useful, in particular in spotting cyclic dependencies, and in showing the amount of complexity at the namespace level. Too much splitting, and there's not a lot there; too little splitting and there's too much there.
AssemblyClasses: This is generally too wide in scope, and shows too much non-related functionality to be useful. Perhaps, it might be useful for small assemblies with an over-eager number of namespaces.
As well as the "inside" linkage of items (i.e., all those items contained by an item), we can also look at "outside" linkage, all those items that use, or are used by, an item. Conceptually, these have a "central" node, and then inward dependencies to the left, outwards, or the right. This can typically form a double fan, so I've called them "hierarchy" diagrams to distinguish them.

- Hierarchy diagrams are more difficult to generate, as to discover the left hand side requires a search of anything that may use the item. This is the equivalent of "grepping" code, searching all source code for hits on an item, and is something your average developer does *a lot*. However, there is an up side, in that we're only interested in files we've opened as part of the host project.

- Assembl yHierarchy
While we already have a plug-in for AllAssemblies, with a large project it can be difficult to see for a single assembly, or slow to produce. This shows assemblies used by an assembly (as does Visual Studio), but also all assemblies that use this assembly (within the project). I've found this useful during build construction and maintenance.

- Class Hierarchy
This tells us who we inherit from, what we implement, who inherits or implements us, any class we use directly in functional fashion, and any class that uses us directly in functional fashion. This is quite useful in conjunction with the ClassMethod diagram - having both open and selecting a class gives you the inside view and the outside view next to each other. It's also an interesting way of navigating around the code. If I could trust that it had found every possible dependence, and if it could jump to lines of code, then it would be more useful, as a possible replacement for some of the "grepping".

- Method Hierarchy
All methods that may call the central method, and all methods that this method calls. Again, this can lead to interesting code navigation.

Modification
--------

![image](https://raw.githubusercontent.com/mayatforest/Refractor/master/Doc/Class%20Call%20Usage.PNG)
