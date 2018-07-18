import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { VictoryBar, VictoryChart, VictoryAxis, VictoryTheme, VictorySharedEvents, VictoryLabel, VictoryPie } from 'victory';
import moment from 'moment';
import { Grid, Row, Col } from 'react-bootstrap';
import { Badge, Well } from 'react-bootstrap';
import { Button, ButtonGroup } from 'react-bootstrap';
import _ from 'lodash';

// Local
import SignalRConnection from '../../infrastructure/SignalRWrapper';
import { Actions } from './AppUsageChartActions';
import SubPieDetailTable from '../AppUsageChart/SubPieDetailTable';
import DayPickerInput from '../DayPickerInput/DayPickerInput';

class AppUsageChart extends Component {
    constructor(props) {
        super(props);

        this.state = {
            processInfoCollection: [],
            subPieData: null,
            currentSubPieInfo: "",
            tableData: []
        };

        this.connectToSignalR();
    }

    async connectToSignalR() {
        const instance = await SignalRConnection.getInstanceAsync();

        instance.listenToHub("ReceiveMessage", this.receiveMessage);
        instance.invokeHub("SendMessage", "user1", "Hello World").catch(err => console.error(err.toString()));
    }

    receiveMessage(user, message) {
        const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        console.log(msg);
    }
    componentWillMount() {
        this.props.requestProcessInfoCollectionForDate(moment());
    }

    componentWillUnmount() {
        SignalRConnection.getInstanceAsync().then(instance => {
            instance.offHub("ReceiveMessage", this.receiveMessage);
        });
    }

    componentWillReceiveProps(nextProps) {
        if (nextProps.processInfoCollection) {
            var result = _.sortBy(_.reduce(nextProps.processInfoCollection, (result, value) => {
                var item = _.find(result, { processName: value.processName });
                if (!item) {
                    result.push({
                        ..._.omit(value, ["windowTitle"])
                    })
                } else {
                    item.seconds += value.seconds;
                }

                return result;
            }, []), e => -e.seconds).slice(0, 5);


            this.setState({ processInfoCollection: result });
        }
    }

    renderSubPie(processName) {
        if (processName === null) {
            this.setState({ subPieData: null });
            return;
        }

        var statistics = _.sortBy(_.filter(this.props.processInfoCollection, (item) => item.processName == processName), item => -item.seconds);
        var totalTime = _.sumBy(statistics, (item) => item.seconds);
        const refinedStatistics = _.reduce(statistics, (result, value) => {
            if (value.seconds / totalTime > (1 / 100)) {
                result.push(value);
            } else {
                var otherText = "Other (less than 1% of total time spent on this process)";
                var other = _.find(result, (item) => item.windowTitle === otherText);
                if (!other) {
                    result.push({
                        ...value,
                        windowTitle: otherText
                    });
                } else {
                    other.seconds += value.seconds;
                }
            }

            return result;
        }, []);
        this.setState({
            subPieData: refinedStatistics,
            tableData: statistics
        });
    }

    setSubPieInfo(subPieInfo) {
        this.setState({
            currentSubPieInfo: subPieInfo
        });
    }

    handleSelect(date) {
        console.log(date);
    }

    handleDayChange(day) {
        this.setState({selectedDay: day});
    }

    render() {
        return (
            <Grid>
                <Row>
                    <ButtonGroup>
                        <Button 
                            bsStyle='primary'
                            onClick={() => {
                            this.props.requestProcessInfoCollectionForDate(moment());
                            }}>Today</Button>
                        <Button onClick={() => {
                            this.props.requestProcessInfoCollectionForDate(moment().set('day', -1));
                        }}>Yesterday</Button>
                        <DayPickerInput onDayChange={(day) => this.props.requestProcessInfoCollectionForDate(day)}/>
                    </ButtonGroup>
                    
                </Row>
                <Row>
                    <Col sm={6}>
                        <VictoryChart
                            height={300}
                            width={500}
                            domainPadding={30}
                            theme={VictoryTheme.material}>
                            <VictoryAxis
                                tickFormat={() => ''} />
                            <VictoryAxis
                                dependentAxis
                                tickFormat={(seconds) => `${seconds}h`} />
                            <VictoryBar
                                domain={{
                                    y: [0, 8]
                                }}
                                style={{
                                    data: { fill: "#c43a31" }
                                }}
                                data={this.state.processInfoCollection}
                                x={(data) => data.processName}
                                y={(data) => _.round(data.seconds / 3600, 2)}
                                animate={{
                                    duration: 1000,
                                    onLoad: { duration: 0 }
                                }}
                                labels={(item) => item.processName}
                                events={[{
                                    target: "data",
                                    eventHandlers: {
                                        onClick: () => {
                                            return [{
                                                target: "data",
                                                eventKey: "all",
                                                mutation: (props) => {
                                                    return null;
                                                }
                                            },
                                            {
                                                target: "data",
                                                mutation: (props) => {
                                                    const fill = props.style && props.style.fill;

                                                    this.renderSubPie(props.datum.processName);
                                                    return {
                                                        style: {
                                                            fill: "black"
                                                        }
                                                    }
                                                }
                                            }]
                                        }
                                    }
                                }]} />
                        </VictoryChart>
                        <SubPieDetailTable
                            data={this.state.tableData} />
                    </Col>
                    <Col sm={6}>
                        {
                            this.state.subPieData &&
                            <React.Fragment>
                                <VictoryPie
                                    labelRadius={100}
                                    style={{
                                        labels: {
                                            fill: "red",
                                            fontSize: 7
                                        }
                                    }}
                                    labels={(data) => `${_.round(data.seconds / 3600, 2)}h`}
                                    data={this.state.subPieData}
                                    x={(data) => data.windowTitle}
                                    y={(data) => data.seconds}
                                    events={[{
                                        target: "data",
                                        eventHandlers: {
                                            onMouseOver: () => {
                                                return [{
                                                    target: "data",
                                                    mutation: (props) => {
                                                        return {
                                                            style: {
                                                                ...props.style,
                                                                fill: "tomato"
                                                            }
                                                        };
                                                    }
                                                }]
                                            },
                                            onMouseOut: () => {
                                                return [{
                                                    target: "data",
                                                    mutation: (props) => null
                                                }]
                                            },
                                            onClick: () => {
                                                return [{
                                                    target: "data",
                                                    mutation: (props) => this.setSubPieInfo(props.datum)
                                                }]
                                            }
                                        }
                                    }]}>
                                </VictoryPie>
                                <Well>
                                    <p>{this.state.currentSubPieInfo.windowTitle} <Badge>{`${_.round(this.state.currentSubPieInfo.seconds / 60) || 0} minutes`} </Badge></p>
                                    <Button bsStyle="info">View details?</Button>
                                </Well>
                            </React.Fragment>
                        }
                    </Col>
                </Row>
            </Grid>
        )
    }
}

export default connect(
    state => state.appUsage,
    dispatch => bindActionCreators(Actions, dispatch)
)(AppUsageChart)