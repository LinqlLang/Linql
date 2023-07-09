Push-Location ./linql-core
python -m build
Pop-Location
pip install ./linql-core
Push-Location ./linql-client
python -m build
Pop-Location
pip install ./linql-client
Push-Location ./linql-server-sqlalchemy
python -m build
Pop-Location
pip install ./linql-server-sqlalchemy

