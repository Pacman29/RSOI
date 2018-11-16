import React from "react";
import PropTypes from "prop-types";
import Job from "../models/job";
import {Card, ListItem} from "react-onsenui";

export default class JobMainView extends React.Component {
    static propTypes = {
        job: PropTypes.instanceOf(Job),
        key: PropTypes.string
    };

    constructor(props) {
        super(props)
    }

    render(){
        return (
            <ListItem key={this.props.key}>
                {JSON.stringify(this.props.job)}
            </ListItem>
        )
    }
}