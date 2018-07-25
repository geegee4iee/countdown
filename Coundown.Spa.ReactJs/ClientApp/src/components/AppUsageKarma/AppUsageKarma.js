import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import _ from 'lodash';
import {Grid, Row, Col} from 'react-bootstrap';
import { Alert } from 'react-bootstrap';

import { Actions } from './AppUsageKarmaActions';
import DayPickerInput from '../DayPickerInput/DayPickerInput';
import KarmaDetailTable from './KarmaDetailTable';


class AppUsageKarma extends Component {
    computeKarmaPoints(karmaResults) {
        return _.reduce(karmaResults, (result, value) => {
            if (value.category === 0) {
                result.good += value.seconds * 1.5;
                result.totalGoodTime += value.seconds;
            }

            if (value.category === 1) {
                result.bad += value.seconds * 2;
                result.totalBadTime += value.seconds;
            }

            if (value.category === 2) {
                result.neutral += value.seconds * 1.25;
                result.totalNeutralTime += value.seconds;
            }

            return result;
        }, {
            good: 0,
            bad: 0,
            neutral: 0,
            totalGoodTime: 0,
            totalBadTime: 0,
            totalNeutralTime: 0
        });
    }

    render() {
        const karmaPoints = this.computeKarmaPoints(this.props.karmaResults);

        let karmaStatus = '';
        let karmaStyle = 'info';

        if (karmaPoints.bad > 40000) {
            karmaStatus = `You're an fucking asshole!!!`
            karmaStyle = 'danger';
        } 
        else if (karmaPoints.bad > 25000) {
            karmaStatus = `You're such a big failure`;
            karmaStyle = 'danger';
        } 
        else if (karmaPoints.bad > 15000) {
            karmaStatus = `What's up? Dickhead?`;
            karmaStyle = 'warning';
        } else if (karmaPoints.good > 0){
            karmaStyle = 'success';
            karmaStatus = `Well! You didn't disappoint me...`;
        }

        return (
            <div>
                
                <Grid>
                    <Row>
                        <Col sm={12}>
                            <DayPickerInput onDayChange={(day) => this.props.requestLastWeekAppUsageRecords(day)}/>
                        </Col>
                    </Row>
                    <Row>
                        <Col
                            sm={6}>
                            <p>
                                Good points: {karmaPoints.good}
                                <br/>
                                Bad points: {karmaPoints.bad}
                                <br/>
                                Neutral points: {karmaPoints.neutral}
                                <br/>
                            </p>
                            <Alert bsStyle={karmaStyle}>
                                <h4>Rating: {karmaStatus}</h4>
                            </Alert>
                        </Col>
                        <Col sm={6}>
                            <p>
                                Total good time: {_.floor(karmaPoints.totalGoodTime / 3600, 2)}h<br/>
                                Total bad time: {_.floor(karmaPoints.totalBadTime / 3600, 2)}h<br/>
                                Total neutral time: {_.floor(karmaPoints.totalNeutralTime / 3600, 2)}h
                            </p>
                        </Col>
                    </Row>
                    <Row>
                        <Col
                            sm={6}>
                                <KarmaDetailTable
                                data={this.props.karmaResults}/>
                        </Col>
                    </Row>
                </Grid>



                
            </div>
        );
    }
}

export default connect(
    state => state.appUsageKarma,
    dispatch => bindActionCreators(Actions, dispatch)
) (AppUsageKarma);