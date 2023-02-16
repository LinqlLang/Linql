// Karma configuration file, see link for more information
// https://karma-runner.github.io/1.0/config/configuration-file.html

const path = require('path');

module.exports = function (config)
{
    config.set({
        basePath: '../../../C#/Test/Linql.Test.Files/TestFiles',
        frameworks: ['jasmine', '@angular-devkit/build-angular'],
        plugins: [
            require('karma-jasmine'),
            require('karma-chrome-launcher'),
            require('karma-jasmine-html-reporter'),
            require('karma-coverage-istanbul-reporter'),
            require('@angular-devkit/build-angular/plugins/karma'),
            require('karma-coverage')
        ],
        client: {
            clearContext: false // leave Jasmine Spec Runner output visible in browser
        },
        coverageReporter: {
            dir: require('path').join(__dirname, '../../../Typescript/coverage/linql.client'),
        },
        coverageIstanbulReporter: {
            dir: require('path').join(__dirname, '../../../Typescript/coverage/linql.client/instanbul'),
            reports: ['html', 'lcovonly', 'text-summary'],
            fixWebpackSourcePaths: true
        },
        jasmineHtmlReporter: {
            dir: require('path').join(__dirname, '../../../Typescript/coverage/linql.client/html'),
        },
        reporters: ['progress', 'kjhtml'],
        port: 9876,
        colors: true,
        logLevel: config.LOG_INFO,
        autoWatch: true,
        browsers: ['Chrome'],
        files: [
            { pattern: "**/*", watched: false, included: false, served: true },

        ],
        proxies: {

        },
        singleRun: false,
        restartOnFileChange: true
    });
};
