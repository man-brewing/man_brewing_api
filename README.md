# M.A.N. Brewing API
This is the backend for data collection and retrieval for the [M.A.N Brewing website](http://man-brewing.ddns.net/).

## Endpoints
The API consists of several endpoints:
* /api - Returns JSON of the most recent data record.
* /api/history/:limit - Returns a JSON array of the number of records specified by `:limit` or the default amount if not specified.
* POST - This is where the [beer room monitor](https://github.com/man-brewing/environment_monitor) sends its data.

## Data Store
Data is stored using [MySQL](https://www.mysql.com/) and accessed via the [Dapper](https://dapper-tutorial.net/dapper).