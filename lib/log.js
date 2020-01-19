const winston = require('winston');
require('dotenv').config();

module.exports = function(filename) {
    const logger = winston.createLogger({
        format: winston.format.combine(
                winston.format.timestamp({format: 'YYYY-MM-DD HH:mm:ss'}),
                winston.format.simple()
        ),
        transports: [
            new winston.transports.File({
                filename: `${filename}.log`,
                level: 'info'
            })            
        ]
    });

    if (process.env.ENV !== 'PRODUCTION') {
        logger.add(new winston.transports.File({
            filename: `${filename}_debug.log`,
            level: 'debug'
        }));

        logger.add(
            new winston.transports.Console({
                level: 'debug'
        }));
    }    

    return logger;
}
