const {
    globSync,
} = require('glob')

const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process')


const ignore = [

]

const registry = process.argv.pop();
const npmArgs = [];

if (registry.startsWith("http"))
{
    console.log(`publishing to registry ${ registry }`);
    npmArgs.push(`--registry ${ registry }`);
}

const packages = globSync('dist/**/package.json', { ignore: ignore });

console.log(packages);


packages.forEach(r =>
{
    const dir = path.dirname(r);
    const command = `npm publish ${ npmArgs.join(" ") }`;
    console.log(dir);
    console.log(command);
    execSync(command, { stdio: 'inherit', cwd: dir });
});
