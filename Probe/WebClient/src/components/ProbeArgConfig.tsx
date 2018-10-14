import * as React from "react";
import { ProbeArgType, ProbeArg } from "./ProbeApiClient";

import "./probeArgConfig.css";

interface ProbeArgConfigProps {
    id: string;
    args: ProbeArg[];
    onValueSet: (name: string, value: string) => void;
}

const ProbeArgConfig: React.SFC<ProbeArgConfigProps> = (props: ProbeArgConfigProps) => {
    const elements: JSX.Element[] = [];
    if (props.args == null || props.args.length < 1) {
        elements.push(<p key={`probe-arg-noctrl-${props.id}`} className="probe-noarg">No arguments</p>);
    } else {
        for(const arg of props.args) {

            let control: JSX.Element;
            switch (arg.type) {
                case ProbeArgType.String :
                    control = <StringArgControl key={`probe-arg-ctrl-${arg.name}`} arg={arg}
                        onValueSet={(name: string, value: string) => props.onValueSet(name, value)} />;
                    break;
                case ProbeArgType.Date :
                    control = <DateArgControl key={`probe-arg-ctrl-${arg.name}`} arg={arg}
                        onValueSet={(name: string, value: string) => props.onValueSet(name, value)} />;
                    break;
                case ProbeArgType.Number :
                    control = <NumberArgControl key={`probe-arg-ctrl-${arg.name}`} arg={arg}
                        onValueSet={(name: string, value: string) => props.onValueSet(name, value)} />;
                    break;
                default:
                    throw Error("Unsupported control type " + arg.type);
            }

            elements.push(
                <div key={`probe-arg-${arg.name}`} className="probe-arg">
                    <label key={`probe-arg-lbl-${arg.name}`}>{arg.name}</label>
                    {control}
                    {arg.isRequired &&
                        <span key={`probe-rqd-${arg.name}`} className="probe-required">&nbsp;</span>
                    }
                </div>
            );
        }
    }

    return (
        <fieldset key={`probe-args-${props.id}`} className="probe-args">
            <legend key={`probe-args-lbl-${props.id}`}>Probe Arguments</legend>
            {elements}
        </fieldset>
    );
}

interface ArgControlProps {
    arg: ProbeArg;
    onValueSet: (name: string, value: string) => void;
}

const StringArgControl: React.SFC<ArgControlProps> = (props: ArgControlProps) => {
    return (
        <input key={`probe-ctrl-${props.arg.name}`} type="text" className="probe-ctrl-string" onBlur={(event) => {
            event.preventDefault();
            props.onValueSet(props.arg.name, event.target.value);
        }} />
    );
};

const DateArgControl: React.SFC<ArgControlProps> = (props: ArgControlProps) => {
    return (
        <input key={`probe-ctrl-${props.arg.name}`} type="date" className="probe-ctrl-date" onChange={(event) => {
            event.preventDefault();
            props.onValueSet(props.arg.name, event.target.value);
        }} />
    );
};

const NumberArgControl: React.SFC<ArgControlProps> = (props: ArgControlProps) => {
    return (
        <input key={`probe-ctrl-${props.arg.name}`} type="number" className="probe-ctrl-number" step="0.001" onChange={(event) => {
            event.preventDefault();
            props.onValueSet(props.arg.name, event.target.value);
        }} />
    );
};

export default ProbeArgConfig;
