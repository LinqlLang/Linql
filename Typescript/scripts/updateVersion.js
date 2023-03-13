const {
    globSync,
} = require('glob')

const fs = require('fs');

const ignore = [
    'node_modules/**',
    "examples/**"
]

const version = process.argv.pop();

const packages = globSync('**/package.json', { ignore: ignore });

console.log(packages);


packages.forEach(r =>
{
    const file = fs.readFileSync(r, 'utf-8');
    const parsed = JSON.parse(file);
    parsed.version = version;
    const updatedFile = JSON.stringify(parsed, null, 2);
    fs.writeFileSync(r, updatedFile);
});
