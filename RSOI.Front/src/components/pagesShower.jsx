import React from "react";
import PropTypes from "prop-types";
import Job from "../models/job";
import {inject, observer} from "mobx-react";
import {computed, action, observable} from "mobx";
import FilesStore from "../stores/FilesStore";
import {
    Card,
    CardHeader,
    Page} from "framework7-react";
import Loader from "./loader";
import JobsStore from "../stores/JobsStore";
import AppNavbar from "./appNavbar";

@inject('jobsStore')
@inject('filesStore')
@observer
export default class PageShower extends React.Component{
    static propTypes = {
        job: PropTypes.instanceOf(Job),
        jobStore: PropTypes.instanceOf(JobsStore),
        filesStore: PropTypes.instanceOf(FilesStore),
        refLink: PropTypes.func
    };

    @observable _pages = [];
    @observable job;

    constructor(props){
        super(props);
        this.jobStore = this.props.jobsStore.getStoreForJobId(this.$f7route.params.jobId);
        if(this.props.job === undefined){
            this.jobStore.getJobInfo()
                .then(action(res => {
                    this.job = res;
                    this.initPages();
                    this.loadPages();
                }))
                .catch(action(e => {
                    console.log(e);
                }))
        } else {
            this.job = this.props.job;
            this.initPages();
        }
    }

    @action initPages(){
        for(let i = 0; i< this.job.pageCount; ++i){
            this._pages.push({
                id: i,
                isLoading: observable.box(true),
                img: observable.box(undefined),
            })
        }
    }

    loadPages(){
        this._pages.forEach(page => {
            this.props.filesStore.getImage(this.job.jobId,page.id).then(action(res => {
                page.img.set(URL.createObjectURL(res));
                page.isLoading.set(false);
            }))
        })
    }

    @action reloadPages = (evt,done) => {
        this.loadPages();
        done();
    };

    componentDidMount(){
        if(this._pages.every(page => page.img.get() !== undefined))
            return;
        this.loadPages();
    }

    componentWillUnmount(){
        this._pages.forEach(page => {
            URL.revokeObjectURL(page.img.get());
        })
    }

    render() {
        return (
            <Page ptr onPtrRefresh={this.reloadPages}>
                <Loader isLoading={this.jobStore.isLoading}>
                    <AppNavbar back={true}/>
                    {
                        this._pages.map((page,idx) =>(
                            <Card key={idx}>
                                <Loader isLoading={page.isLoading.get() || page.img.get() === undefined}>
                                    <CardHeader>
                                        <img style={{maxWidth: "100%"}} src={page.img.get()}/>
                                    </CardHeader>
                                </Loader>
                            </Card>
                            )
                        )
                    }
                </Loader>
            </Page>
        );
    }

}