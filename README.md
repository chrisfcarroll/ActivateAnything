# ActivateAnything

```
Activate.New<T>()
``` 
…will make an extreme effort to find and instantiate a concrete class which is assignable to a `Type`, whether or not the Type is concrete, whether or not the Type has constructor dependencies, whether or not a suitable concrete subtype is found in any currently loaded `Assembly`.

The attempt to find and instantiate suitable Types is driven by three kinds of `IActivateAnythingRule`.

- `IActivateInstanceRule` simply returns an instance of a concrete type.
- `IFindTypeRule` provides rules for where to look for candidate concrete subtypes of an abstract type
- `IChooseConstructorRule` rules for how to choose between constructors when a concrete `Type` has been chosen.

The <em>extremity</em> of the effort lies in its recursive attempt to find and construct the <em>dependencies</em>.

The default ruleset will
- Search first the currently executing assembly, then its references, then its base directory, then the current working directory
- Prefer constructors with more parameters over constructors with fewer.

Customise or replace the default ruleset with the provided rules or custom rules.    

### Example Usage
See [FixtureBase](https://www.nuget.org/packages/FixtureBase) : reduce the cost of Unit Testing by automating setup and the injection of fakes.
