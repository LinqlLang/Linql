const { execSync } = require('child_process')
const library = process.argv.pop();
execSync(`ng build ${ library }`, { stdio: 'inherit' });