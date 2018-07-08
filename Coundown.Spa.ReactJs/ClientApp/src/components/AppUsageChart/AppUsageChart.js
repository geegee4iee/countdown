import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { VictoryBar, VictoryChart, VictoryAxis, VictoryTheme } from 'victory';
import { Grid, Row, Col } from 'react-bootstrap';
import _ from 'lodash';

// Local
import { Actions } from './AppUsageChartActions';

class AppUsageChart extends Component {
    constructor(props) {
        super(props);

        this.state = {
            processInfoCollection: []
        }
    }
    componentWillMount() {
        this.props.requestTodayProcessInfoCollection();
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (this.props.processInfoCollection !== nextProps.processInfoCollection) {
            return true;
        }

        return false;
    }

    componentWillReceiveProps(nextProps) {
        if (nextProps.processInfoCollection) {
            var result = _.sortBy(_.reduce(nextProps.processInfoCollection, (result, value, key) => {
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

    render() {
        return (
            <Grid>
                <Row>
                    <VictoryChart
                        domainPadding={20}
                        theme={VictoryTheme.material}>
                        <VictoryAxis
                        />
                        <VictoryAxis
                            dependentAxis
                            tickFormat={(x) => `${_.round(x / (60 * 60), 2)}h`}
                        />
                        <VictoryBar
                            data={this.state.processInfoCollection}
                            x="processName"
                            y="seconds" />
                    </VictoryChart>
                </Row>
            </Grid>
        )
    }
}

export default connect(
    state => state.appUsage,
    dispatch => bindActionCreators(Actions, dispatch)
)(AppUsageChart)