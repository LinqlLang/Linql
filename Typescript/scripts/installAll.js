const { execSync } = require('child_process')

const {
    globSync,
} = require('glob')

const fs = require('fs');

const ignore = [
    '**/node_modules/**',
    "examples/**",
    "dist/**",
    "package.json"
]

let packages = globSync('**/package.json', { ignore: ignore });

console.log(packages);

packages = packages.sort((left, right) =>
{
    const leftFile = fs.readFileSync(left, 'utf-8');
    const rightFile = fs.readFileSync(right, 'utf-8');
    const leftParsed = JSON.parse(leftFile);
    const rightParsed = JSON.parse(rightFile);

    const leftDeps = leftParsed.devDependencies;
    const rightDeps = rightParsed.devDependencies;

    if (leftDeps && leftDeps[rightParsed.name])
    {
        return 1;
    }
    else if (rightDeps && rightDeps[leftParsed.name])
    {
        return -1;
    }
    return 0;

});

console.log(`Order: \n${ packages.join("\n") }`);

packages.forEach(r =>
{
    const file = fs.readFileSync(r, 'utf-8');
    const parsed = JSON.parse(file);
    execSync(`npm run setup ${ parsed.name }`, { stdio: 'inherit' });
});

