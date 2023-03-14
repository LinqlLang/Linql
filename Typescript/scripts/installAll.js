const { execSync } = require('child_process')

const {
    globSync,
} = require('glob')

const fs = require('fs');

const ignore = [
    '**/node_modules/**',
    "examples/**"
]

const packages = globSync('**/package.json', { ignore: ignore });

console.log(packages);

packages.forEach(r =>
{
    const file = fs.readFileSync(r, 'utf-8');
    const parsed = JSON.parse(file);
    execSync(`npm run install ${ parsed.name }`, { stdio: 'inherit' });
});

