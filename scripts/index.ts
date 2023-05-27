#!/usr/bin/env ts-node

import path from 'path';
import {config} from 'dotenv';
import {spawn} from 'child_process';

console.log("--- App localization util: preparing ---\n")

const configFilePath = '.env.development';
console.log(`Reading ${configFilePath} file and global config...`);
const env = config({path: configFilePath});

function getEnv(key: string, defaultValue: string | undefined = undefined): string {
    let value: string | undefined;
    if (env.parsed) {
        value = env.parsed[key];
    }
    if (!value) {
        value = process.env[key];
    }
    if (!value) {
        value = defaultValue;
    }
    if (!value) {
        throw new Error(`Env value not found for key ${key}`);
    }
    return value;
}

function runCmd(cmd: string, args: string[]) {
    return new Promise<void>((resolve) => {
        // eslint-disable-next-line global-require
        const child = spawn(cmd, args);

        child.stdout.on('data', (buffer: any) => {
            console.log(buffer.toString());
        });
        child.stdout.on('end', () => {
            resolve();
        });
    });
}

const configFileEnvName = "APP_LOCALIZATION_UTIL_CONFIG";
const configFileEnvDefault = ".localization-config.json";
const configFile = getEnv(configFileEnvName, configFileEnvDefault);
console.log("configFile:", configFile);

console.log();

const projectPath = path.resolve(__dirname, '..');

if (projectPath) {
    runCmd('dotnet', ['run', '--project', projectPath, '--ConfigFile', configFile]);
}
