require('dotenv').config();
const express = require('express');
const mysql = require('mysql');
const bodyParser = require('body-parser')
const fetch = require("node-fetch");
const winston = require('winston');
const path = require('path');

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
 * Connect to the MySQL database.
 */
con.connect(function (err) {
    if (err) throw err;
    logger.info('Database connected');
});

/**
 * Make the default logger.
 */
const logger = winston.createLogger({
    format: winston.format.combine(
            winston.format.timestamp({format: 'YYYY-MM-DD HH:mm:ss'}),
            winston.format.simple()
    ),
    transports: [
        new winston.transports.File({
            filename: path.join(__dirname, '../../logs', 'beerroom_api.log'),
            level: process.env.LOG_LEVEL
        })            
    ]
});

/** 
 * Add a console logger if we are not in production.
 */
if (process.env.ENV !== 'PRODUCTION') {
    logger.add(
        new winston.transports.Console({
            level: process.env.LOG_LEVEL
    }));
}

/**
 * Enable CORS for cross domain API requests.
 */
app.use(function(req, res, next) {
    res.header('Access-Control-Allow-Origin', '*');
    res.header('Access-Control-Allow-Headers', 'Origin, X-Requested-With, Content-Type, Accept');
    next();
});
  

/**
 * Basic GET request sends back latest weather data.
 */
app.get('/', function (req, res) {
    logger.debug('GET');

    let latestTempData = 'SELECT ambient_temp, ambient_humid, timestamp FROM DataLog ORDER BY TIMESTAMP ASC LIMIT 1';
    con.query(latestTempData, function (err, result) {
        if (err) throw err;

        logger.debug('selected data from mysql', result[0]);
    
        res.status(200).json({ status: '200', temp: result[0].ambient_temp, humidity: result[0].ambient_humid, timestamp: result[0].timestamp });
    });
});

/**
 * Redirect to history with default limit.
 */
app.get('/history', function (req, res) {
    logger.debug('Redirect history');
    res.redirect('history/5');
});

/**
 * Get the most recent weather data, limiting to the specified number.
 */
app.get('/history/:limit', function (req, res) {
    logger.debug('GET history; params: ' + req.params.limit);

    let limit = !isNaN(req.params.limit) ? Number.parseInt(req.params.limit) : 100;

    let latestTempData = `SELECT temperature, humidity, ambient_temp, ambient_humid, timestamp FROM DataLog ORDER BY TIMESTAMP DESC LIMIT ${mysql.escape(limit)} ORDER BY timestamp ASC`;

    logger.debug('sql ' + latestTempData);

    con.query(latestTempData, function (err, result) {
        if (err) throw err;
    
        res.status(200).json({ results: result});
    });
});

app.get('/history/:startDate/:endDate', function (req, res) {
    logger.debug(`GET history; params: ${req.params.startDate} ${req.params.endDate}`);

    let timeboxEnvironmentQuery = `SELECT temperature, humidity, ambient_temp, ambient_humid, timestamp FROM DataLog WHERE timestamp BETWEEN ${mysql.escape(req.params.startDate)} AND ${mysql.escape(req.params.endDate)} ORDER BY timestamp ASC;`;

    logger.debug('sql ' + timeboxEnvironmentQuery);

    con.query(timeboxEnvironmentQuery, function (err, result) {
        if (err) throw err;
        
        res.status(200).json({ results: result});
    });
});

/**
 * Handles POST requests from the environment monitor and
 * saves the data to the database.
 */
app.post('/', (req, res) => {
    logger.debug('POST');

    fetch(`https://api.openweathermap.org/data/2.5/weather?id=${process.env.WEATHER_CITY_ID}&appid=${process.env.WEATHER_API_KEY}&units=metric`)
        .then((res) => res.json())
        .then((data) => {
            let ambient_temp = 0.00;
            let ambient_humid = 0.00;

            logger.debug('got weather data');
    
            ambient_temp = data.main.temp;
            ambient_humid = data.main.humidity;
            
            // save those data to the database
            let insertQuery = 'INSERT INTO DataLog (temperature, humidity, ambient_temp, ambient_humid) VALUES (' + mysql.escape(req.body.temp) + ', ' + mysql.escape(req.body.humidity) + ', ' + mysql.escape(ambient_temp) + ', ' + mysql.escape(ambient_humid) + ')';
            con.query(insertQuery, function (err, result) {
                if (err) throw err;

                if (result.affectedrows < 1) {
                    logger.error('Something went wrong inserting new environment record');
                }

                logger.debug('inserted data to mysql');

                res.status(200).send("received");
            });
        })
        .catch(error => {
            logger.error('an error occurred getting weather data', error.message);

            res.status(500).json({ status: '500', message: error.message});
        });
});

/**
 * Catch 404 and forward to error handler
 */
app.use(function (req, res, next) {
    logger.error('404 Not Found');
    logger.error(req.protocol);
    logger.error(req.hostname);
    logger.error(req.path);
    logger.error(req.originalUrl);
    logger.error(req.method);

    let err = new Error('Not Found');
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

        res.status(err.status || 500).send(err);
    });
}

/**
 * Production error handler, no stacktraces leaked to user
 */
app.use(function (err, req, res, next) {
    logger.error(err);
    logger.error(req);

    res.status(err.status || 500).json({ message: "This request was not handled." });
});

/**
 * Starts the server on the configure port and starts listening for requests.
 */
var server = app.listen(app.get('port'), function () {
    logger.info('Express server listening on port ' + server.address().port);
});
