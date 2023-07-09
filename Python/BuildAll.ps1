Push-Location ./linql-core
python -m build
Pop-Location
pip install ./linql-core
Push-Location ./linql-client
python -m build
Pop-Location
pip install ./linql-client
