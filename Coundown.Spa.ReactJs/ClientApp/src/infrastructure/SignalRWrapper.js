import { HubConnectionBuilder } from '@aspnet/signalr';

let signalRInstance = null;
const constructorToken = Symbol('signalr-constructor');

const connectToHub = () => {
    const connection = new HubConnectionBuilder().withUrl("https://localhost:5001/realtime").build();
    connection.start().then(e => {
        console.log("Connected to the hub");
    }).catch(err => console.error(err.toString()));

    return connection;
}

export default class SignalRConnection {
    constructor(symbol) {
        if (symbol !== constructorToken) {
            throw new Error("Cannot initialize object manually!!");
        }

        this.signalRConnection = null;
    }

    static getInstanceAsync = () => {
        return new Promise((resolve, reject) => {
            if (signalRInstance == null) {
                signalRInstance = new SignalRConnection(constructorToken);
                signalRInstance.signalRConnection = new HubConnectionBuilder().withUrl("https://localhost:5001/realtime").build();

                signalRInstance.signalRConnection.start().then(e => {
                    console.log("Connected to hub successfully, resolving...!");
                    resolve(signalRInstance);
                }).catch(err => {
                    console.error(err.toString())
                    reject(err);
                });
            } else {
                resolve(signalRInstance);
            }
        })
    }

    listenToHub = (methodName, func) => {
        this.signalRConnection.on(methodName, func);
    }

    invokeHub = (methodName, ...args) => {
        return this.signalRConnection.invoke(methodName, ...args);
    }

    offHub = (methodName, func) => {
        return this.signalRConnection.off(methodName, func);
    }
}