
export class TestFileLoader 
{
    constructor(protected BasePath: string = "")
    {

    }

    async GetFile(FileName: string): Promise<string>
    {
        return await fetch(`base/${ this.BasePath }/${ FileName }.json`).then(r => r.text());
    }
}