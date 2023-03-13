const { execSync } = require('child_process')
const library = process.argv.pop();

const path = `projects/${ library }`;
execSync(`npm i`, { stdio: 'inherit' });
execSync(`npm i`, { stdio: 'inherit', cwd: path });
execSync(`npm run build ${ library }`, { stdio: 'inherit' });