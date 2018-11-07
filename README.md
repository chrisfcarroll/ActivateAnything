# ActivateAnything

Pre-alpha, under-construction.

`ActivateAnything.CeateInstance.Of<T>()` will make an extreme best effort to find and instantiate a concrete class which is assignable to `Type`. Guide it with two kinds of rule:


- Rules for where, and in what order, to look for concrete types assignable to the target Type
- Rules for choosing between constructors

The default ruleset will
- Search first the currently executing assembly, then its references, then its base directory, then the current working directory
- Prefer constructors with more parameters over constructors with fewer.

The <em>extremity</em> of the effort lies in its recursive attempt to find and construct the <em>dependencies</em> implied by rule 2.
