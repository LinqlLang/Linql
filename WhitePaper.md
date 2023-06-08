# Linql: A Next-Generation, Language-Native Graph Api Language
##### Kris Sodroski (2023)  

## Introduction 


Since the inception of the internet, computer to computer communications have been the cornerstone of technological advancement.  Up until the 1990s, network communications were complex and almost exclusively limited to advanced users, corporations, and governments.  

The introduction of interactive websites - programmed in `javascript` and communicating over `HTTP` -  ushered in the `cyber revolution`.  By hiding complex communication protocols behind graphical user interfaces (`GUIs`) connected to web services (`APIs`), `web application` architecture has inanarguably been foundational to humankind's rapid progress.  While `web application` implementations and programming principals have undoubtably been successful, their influence has also created significant communication fragmentation and inefficiencies across all of computing. 

Over the years, there have been many attempts at standardizing communication protocols, each with their own advantages and disadvantages.  This paper will explore many of these protocols from the `perspective of the author`, and propose an alternative protocol, `Linql`, that aims to provide significant advantages and efficiencies over existing methodolgies.

## The Beginnings - XHR Requests

The beginning years of `web applications` relied heavily on `XHR`, typically utilizing `XML` based requests and responses to drive their web `GUIs`.  Lacking any best practices, `APIs` designed to receive and deliver this data typically contained individually crafted request and response structures and endpoints.

Over the years, architectural tastes changed, such as preferring `JSON` over `XML`, standardizing data delivery, but `custom endpoint implementations` continued to reign, ultimately continuing communication fragmentation. 

## The Silver Bullet - REST 

In the early 2000s, representational state transfer (`REST`) was introduced in an attempt to standardize request and response design across the web.  While fundamentally helpful as a guideline, `REST`'s non-specific constraints caused many unintended consequences.    

`REST` emphasizes that compliant servers should have a `uniform interface` defined by [four constraints](https://en.wikipedia.org/wiki/Representational_state_transfer#Uniform_interface).  Of these constraints, `resource identification in requests` has had the most detrimental impact on interoperability across the web. 

Without an explicitly stated implementation, `REST` architectures evolved to mirror `HTTP` resource requests, with `data access` exclusively being performed through `HTTP GET` with user defined `filters` in the `query string`.

Despite `REST`'s efforts to reduce complexity and create a generic multipurpose interface, the complexity of modern web capabilities elucidate foundational issues with it's design. 

### 1. Primary Key Filtering 

At first glance, `resource identification in requests` seems to provide a common interface for `Types` based on their `uniqueness`. 

Imagine I have the following data model with a `unique identifier`: 

```typescript
class TestObject 
{
    ID: number;
}
```

Filtering to get a specific `TestObject` by it's `ID` is relatively simple.

```powershell
curl GET /TestObject/{ID}
```

By adding a `composite unique identifier`, a common scenario for complex applications, a significant issue arrises. 

```typescript
class TestObject 
{
    ID-1: number;
    ID-2: number;
}
```

In this case, which is the correct order? 

```powershell
curl GET /TestObject/{ID-1}/{ID-2}
curl GET /TestObject/{ID-2}/{ID-1}
```

While extreme, imagine there were `N` unique identifiers.  How would `REST` handle this situation? 

```typescript
class TestObject 
{
    ID-1: number;
    ID-2: number;
    ...
    ID-N: number;
}
```

```powershell
curl GET /TestObject/{ID-1}/{ID-2}/.../{ID-N}
```

This request format requires clients to be aware of the data model's internal implementation while also coupling request complexity to uniqueness complexity. 

### 2. Language-Dependant Search

Searching data is generally implemented through the `plural` endpoint.  

```powershell
curl GET /TestObjects
```

These plural endpoints create inconsistencies based on the language of the developers.

```powershell
curl GET /TestObjects
curl GET /Mouses <-- Nonstandard plural
curl GET /Boxes  <-- Nonstandard plural
```

### 3. Inadiquet Search

`Nonunique properties` are generally appended as `Query Parameters` in `REST`.  Let's take our TestObject from before and modify it.

```typescript
class TestObject 
{
    ID: number;
    TestObjectName: string;
}
```

Filtering on TestObjectName `equals` some `Value` is relatively simple.  

```powershell
curl GET /TestObjects?TestObjectName={Value}
```

But filtering data where `TestObjectName` `contains` some `Value` is illdefined by `REST`.  This has lead to many custom implementations, such as `LHS Brackets` and `RHS Colon`.

```powershell
curl GET /TestObjects?TestObjectName[contains]={Value}&ID[gte]=4
```

Despite these efforts, most modern applications would benefit from even more advanced filtering capabilities, such as searching through `nestings` and `lists`.

```typescript
class TestObject 
{
    ID: number;
    NestedObject: TestObject;
    ArrayObject: Array<TestObject>;
}
```

Filtering TestObjects by `NestedObject` and/or `ArrayObject` is many times impossible to accomplish with `REST` architectures.

### 4. URL Max Length Limitations

`REST`'s insistence of placing so much information in the url makes complex searching unreliable and unmaintainable, if not completely impossible due to [URL Length Limitations](https://stackoverflow.com/questions/417142/what-is-the-maximum-length-of-a-url-in-different-browsers).  

### 5. One-to-One Development

Due to insufficient capabailities, `Services` have generally been developed to contain many endpoints, each designed to satisify custom searching.

For one, development teams generally create a controller that at minimum, contains surrogates for the HTTP methods.  Additional methods are then added on to support common usecases that `REST` cannot inherently support.  


#### **`Typical Controller`**
```csharp
public class TestObjectController : Controller 
{
    public TestObject Get(int PrimaryKey) {};

    public TestObject Put(TestObject Object) {};

    public TestObject Delete(TestObject Object) {};

    public TestObject Post(TestObject Object) {};

    public TestObject GetObjectByNestedObjectID(int NestedObjectID) {};

    public TestObject GetObjectByNestedObjectName(string NestedObjectName) {};

}
```

An example of this architecture can be seen in the [Azure Dev Ops Repositories endpoint](https://learn.microsoft.com/en-us/rest/api/azure/devops/git/repositories?view=azure-devops-rest-7.1), which contains multiple endpoints for specific use cases.

As developers couple controllers, types, and actions in a one-to-one configuration, systems grow to unprecedented sizes and quickly become unmaintainable, clunky, and confusing.    
### 6. N+1 Data Requests

### 7. Request Queue Bottleneck 