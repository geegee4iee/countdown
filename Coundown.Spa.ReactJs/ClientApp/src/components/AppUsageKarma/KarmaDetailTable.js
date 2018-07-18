import React, { Component } from 'react';
import ReactTable from 'react-table';
import 'react-table/react-table.css';
import _ from 'lodash';

export default class KarmaDetailTable extends Component {
    render() {
        const data = this.props.data || [];

        const columns = [{
            Header: 'Title',
            accessor: 'title'
        }, 
        {
            Header: 'Process Name',
            accessor: 'process',
        }, {
            Header: 'Karma',
            accessor: 'category',
            Cell: props => {
                let text = 'Good';
                const style = {
                    color: 'green'
                };

                if (props.value === 1) {
                    style.color = 'red';
                    text = 'Bad';
                }

                if (props.value === 2) {
                    style.color = 'blue'
                    text = 'Neutral';
                }

                return (
                    <span
                        style={style}>
                        {text}
                    </span>
                )
            }
        }]

        return (
        <ReactTable
            data={data}
            columns={columns}/>
        );
    }
}