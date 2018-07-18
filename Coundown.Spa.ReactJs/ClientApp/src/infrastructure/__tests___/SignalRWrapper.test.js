import SignalRWrapper from '../SignalRWrapper';

describe('Instantiate SignarlRWrapper', () => {
    it('Cannot initialize wrapper manually', () => {
        expect(() => {
            new SignalRWrapper()
        }).toThrow();
    });

    it('Cannot initualize wrapper manually with symbol having the same str', () => {
        expect(() => {
            new SignalRWrapper(Symbol('signalr-constructor'))
        }).toThrow();
    });
})
