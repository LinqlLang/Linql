from sqlalchemy.orm import DeclarativeBase, mapped_column
from sqlalchemy import MetaData, Integer, String, Numeric

metadata_obj = MetaData(schema="test")

class TestBase(DeclarativeBase):
    pass

class Building(TestBase):

    __tablename__ = "Building"
    BuildingID: int = mapped_column("int", Integer, primary_key=True)
    BuildingName: str = mapped_column(String(50), nullable=False)
    Latitude: float = mapped_column(Numeric, nullable=False)
    Longitude: float = mapped_column(Numeric, nullable=False)
