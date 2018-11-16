import React from "react";
import PropTypes from "prop-types";
import {inject, observer} from "mobx-react";
import DataStore from "../stores/dataStore";
import JobMainView from "./jobMainView";
import {observable} from "mobx";
import Job from "../models/job";
import {List, ListItem} from "react-onsenui";

@inject('dataStore')
@observer
export default class MainPage extends React.Component {
    static propTypes = {
        dataStore: PropTypes.instanceOf(DataStore),
        jobs: PropTypes.arrayOf(PropTypes.instanceOf(Job))
    };

    constructor(props) {
        super(props);
    }

    componentDidUpdate(){
    }

    render(){
        return (
            <List modifier="longdivider" dataSource={this.props.jobs} renderRow={(job,index) => (<JobMainView key={index} job={job}/>)}/>
        );
    }
}