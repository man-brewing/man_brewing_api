require('dotenv').config();
const express = require('express');
const logger  = require('../lib/log.js')('beerroom');
var mysql = require('mysql');
var bodyParser = require('body-parser')
const fetch = require("node-fetch");

var app = express();

// parse application/x-www-form-urlencoded
app.use(bodyParser.urlencoded({ extended: false }))

// parse application/json
app.use(bodyParser.json())

// port config
app.set('port', process.env.PORT || 3000);

// Configure the database connection
var con = mysql.createConnection({
    host: process.env.DB_HOST,
    user: process.env.DB_USER,
    password: process.env.DB_PASS,
    database: process.env.DB_NAME
});

/**
 * No GETS.
 */
app.get('/', function (req, res) {
    logger.debug('GET');

    res.setHeader('Content-Type', 'application/json');
    res.status(404);
    res.send({ status: '404', message: 'get lost, chump'});
});

/**
 * Connect to the MySQL database.
 */
con.connect(function (err) {
    if (err) throw err;
    logger.debug('Database connected');
});

/**
 * Handles POST requests from the environment monitor and
 * saves the data to the database.
 */
app.post('/beerroom/environment', (req, res) => {
    logger.debug('POST');

    fetch(`https://api.openweathermap.org/data/2.5/weather?id=${process.env.WEATHER_CITY_ID}&appid=${process.env.WEATHER_API_KEY}&units=metric`)
        .then((res) => res.json())
        .then((data) => {
            var ambient_temp = 0.00;
            var ambient_humid = 0.00;

            logger.debug('got weather data');
    
            ambient_temp = data.main.temp;
            ambient_humid = data.main.humidity;
            
            // save those data to the database
            var insertQuery = 'INSERT INTO DataLog (temperature, humidity, ambient_temp, ambient_humid) VALUES (' + mysql.escape(req.body.temp) + ', ' + mysql.escape(req.body.humidity) + ', ' + mysql.escape(ambient_temp) + ', ' + mysql.escape(ambient_humid) + ')';
            con.query(insertQuery, function (err, result) {
                if (err) throw err;

                if (result.affectedrows < 1) {
                    logger.error('Something went wrong inserting new environment record');
                }

                logger.debug('inserted data to mysql');

                res.status(200);
                res.send("received");
            });
        })
        .catch(error => {
            logger.error('an error occurred getting weather data', error.message);

            res.setHeader('Content-Type', 'application/json');
            res.status(404);
            res.send({ status: '404', message: error.message});
        });
});

/**
 * Catch 404 and forward to error handler
 */
app.use(function (req, res, next) {
    logger.error('404 Not Found');
    logger.error(req);

    var err = new Error('Not Found');
    err.status = 404;
    next(err);
});

/**
 * Development error handler, will print stacktrace
 */
if (process.env.ENV === 'DEVELOPMENT') {
    app.use(function (err, req, res, next) {
        logger.error(err);
        logger.error(req);

        res.status(err.status || 500);
        res.send(err);
    });
}

/**
 * Production error handler, no stacktraces leaked to user
 */
app.use(function (err, req, res, next) {
    logger.error(err);
    logger.error(req);

    res.status(err.status || 500);
    res.send('error');
});

/**
 * Starts the server on the configure port and starts listening for requests.
 */
var server = app.listen(app.get('port'), function () {
    logger.info('Express server listening on port ' + server.address().port);
});
