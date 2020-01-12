const winston = require('winston');



// /**
//  * Configures the logger for this application.
//  */
// const logger = winston.createLogger({
//     format: winston.format.combine(
//             winston.format.timestamp({format: 'YYYY-MM-DD HH:mm:ss'}),
//             winston.format.simple()
//     ),
//     transports: [
//         new winston.transports.Console({
//             level: 'debug'
//         }),
//         new winston.transports.File({
//             filename: 'beerroom.log',
//             level: 'info'
//         }),
//         new winston.transports.File({
//             filename: 'beerroom_debug.log',
//             level: 'debug'
//         })
//     ],
//     level: process.env.LOG_LEVEL
// });

// module.exports = logger;

module.exports = function(filename) {
    const logger = winston.createLogger({
        format: winston.format.combine(
                winston.format.timestamp({format: 'YYYY-MM-DD HH:mm:ss'}),
                winston.format.simple()
        ),
        transports: [
            new winston.transports.Console({
                level: 'debug'
            }),
            new winston.transports.File({
                filename: `${filename}.log`,
                level: 'info'
            }),
            new winston.transports.File({
                filename: `${filename}_debug.log`,
                level: 'debug'
            })
        ],
        level: process.env.LOG_LEVEL
    });

    return logger;
}
