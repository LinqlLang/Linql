from linql_client import LinqlSearch

def test_haversine():
    assert LinqlSearch.haversine(52.370216, 4.895168, 52.520008,
    13.404954) == 945793.4375088713