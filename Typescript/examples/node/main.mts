import { Main } from './app.mjs';
process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = '0';

(async () =>
{
    try
    {
        await Main();
    } catch (e)
    {

        console.error(e);
    }
})();