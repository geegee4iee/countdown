import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Counter from './components/Counter';
import FetchData from './components/FetchData';

import AppUsageChart from './components/AppUsageChart/AppUsageChart';
import AppUsageReport from './components/AppUsageReport/AppUsageReport';
import AppUsageKarma from './components/AppUsageKarma/AppUsageKarma';

export default () => (
  <Layout>
    <Route exact path='/' component={Home} />
    <Route path='/counter' component={Counter} />
    <Route path='/fetchdata/:startDateIndex?' component={FetchData} />
    <Route path='/appusage' component={AppUsageChart} />
    <Route path='/appusage-report' component={AppUsageReport} />
    <Route path='/karma' component={AppUsageKarma}/>
  </Layout>
);
