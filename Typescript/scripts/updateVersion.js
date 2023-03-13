const {
    globSync,
} = require('glob')

const fs = require('fs');

const ignore = [
    'node_modules/**',
    "examples/**"
]

const version = process.argv.pop();

if (!version)
{
    console.error("Version format was null or undefined");
    return;
}
if (version.length < 1)
{
    console.error('Version was empty');
    return;
}

const versionCheck = version[0];


const startsWithNumber = Number(versionCheck);

if (!startsWithNumber)
{
    console.error("Version didn't start with a number");
    return;
}

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
