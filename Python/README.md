# Linql

`Linql` is a next generation graph api library.  It's primary goal is to provide language-native graph api integration to achieve consistency and scale across the web. 

These repos focus on Python implementations.  

[Linql Overview](../README.md)

## Example Usage

#### **`Python`**
```python
from functional import seq

statesICareAbout = seq("al", "ma"])
search.Where(lambda r => statesICareAbout.map(lambda t => t.upper().exists(r))).to_list();
```

dev stuff: 

[packaging docs](https://packaging.python.org/en/latest/tutorials/packaging-projects/)
[testing docs](https://docs.python.org/3/library/test.html)

Many thanks to [aprimc](https://github.com/aprimc) for showing me how to parse the bytecode with his project [from_lambda](https://github.com/aprimc/from_lambda/blob/master/README.md)

hatch has no mono repo support.  Really? 