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

In the early 2000s, representational state transfer (`Rest`) was introduced in an attempt to standardize request and response design across the web.  While fundamentally helpful as a guideline, `Rest`'s non-specific constraints caused many unintended consequences.    

`Rest` emphasizes that compliant servers should have a `uniform interface` defined by [four constraints](https://en.wikipedia.org/wiki/Representational_state_transfer#Uniform_interface).  Of these constraints, `resource identification in requests` has had the most detrimental impact on interoperability across the web. 

Without an explicitly stated implementation, `Rest` architectures evolved to mirror `HTTP` resource requests, with `data access` exclusively being performed through `HTTP GET` with user defined `filters` in the `query string`.

Despite `Rest`'s efforts to reduce complexity and create a generic multipurpose interface, the complexity of modern web capabilities elucidate foundational issues with it's design. 

### Limited Filtering Capabilities

At first glance, `resource identification in requests` seems to provide an common interface for `Types`. 

Imagine I have the following data model: 

```typescript
class User 
{
    UserID: number;
    UserName: string;
}
```

Filtering to get a specific `User` by their `UserID` is relatively simple.

> `GET` /User/{`UserID`}




### Duplicate Use of HTTP Methods

### URL Max Length Limitations

#### One-to-One Development

#### N+1 Data Requests

#### Request Queue Bottleneck 