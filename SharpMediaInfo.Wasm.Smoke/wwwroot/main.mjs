// Node entry point: node bin/<cfg>/net10.0/browser-wasm/AppBundle/main.mjs
import { dotnet } from './_framework/dotnet.js';

const exitCode = await dotnet.run();
process.exit(exitCode);
