Alter table test.Building 
drop column Geography; 

Alter table test.Building
drop column Geometry; 

Alter table test.Building 
add Geography as geography::Point(Latitude, Longitude , 4326);

Alter table test.Building 
add Geometry as geometry::Point(Longitude , Latitude, 4326);