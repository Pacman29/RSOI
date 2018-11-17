import React from "react";
import PropTypes from "prop-types";
import Job from "../models/job";
import JobCard from "./jobCard";

export default class JobsList extends React.Component{
    static propTypes = {
        jobs: PropTypes.arrayOf(PropTypes.instanceOf(Job))
    };

    constructor(props){
        super(props)
    }

    render(){
        return(
            this.props.jobs.map((job,idx) => (
                <JobCard key={idx} job={job}/>
            ))
        )
    }
}