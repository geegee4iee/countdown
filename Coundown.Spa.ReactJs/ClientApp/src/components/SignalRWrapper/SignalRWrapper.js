import React, { Fragment } from 'react';
import { HubConnectionBuilder } from '@aspnet/signalr';

const SignalRContext = React.createContext();
const signalRConnection = new HubConnectionBuilder().withUrl("https://localhost:44377/chatHub").build();
export default class SignalRWrapper extends React.Component {
    constructor(props) {
        super(props);

        this.connectToHub();
    }

    static defaultProps = {
        hubUrl: "https://localhost:44377/chatHub"
    }

    componentWillUnmount() {
        this.connection.stop();
    }

    connectToHub() {
        signalRConnection.start().then(e => {
            console.log("Connected to the hub");
        }).catch(err => console.error(err.toString()));
    }

    listenToHub = (methodName, func) => {
        signalRConnection.on(methodName, func);
    }

    invokeHub = (methodName, ...args) => {
        return signalRConnection.invoke(methodName, ...args);
    }

    offHub = (methodName, func) => {
        return signalRConnection.off(methodName, func);
    }

    render() {
        return (
            <SignalRContext.Provider value={{
                listenToHub: this.listenToHub,
                invokeHub: this.invokeHub,
                offHub: this.offHub
            }}>
                {this.props.children}
            </SignalRContext.Provider>
        );
    }
}

export function consumeSignalR(Component) {
    return function SignalRConsumer(props) {
        return (
            <SignalRContext.Consumer>
                {context => <Component {...props} signalRContext={context} />}
            </SignalRContext.Consumer>
        );
    }
    
}