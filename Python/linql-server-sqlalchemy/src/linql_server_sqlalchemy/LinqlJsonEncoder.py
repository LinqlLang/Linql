import json
from linql_core.LinqlSearch import LinqlSearch


class LinqlJsonEncoder(json.JSONEncoder):

    # overload method default
    def default(self, obj):

        # Match all the types you want to handle in your converter
        if isinstance(obj, LinqlSearch):
            return json.JSONEncoder().encode({
                "Type": self.default(obj.Type)
        })
        # Call the default method for other types
        return json.JSONEncoder.default(self, obj)