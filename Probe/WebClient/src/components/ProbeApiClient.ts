export enum ProbeArgType {
    String = 0,
    Date = 1,
    Number = 2,
    DateTime = 3
}

export interface ProbeArg {
    name: string;
    type: ProbeArgType;
    isRequired: boolean;
}
export interface ProbeInfo {
    id: string;
    description: string;
    args: ProbeArg[];
}

export interface ProbeRunArgs {
    [key: string]: string;
}

export interface IProbeApiClient {
    getProbes: () => Promise<ProbeInfo[]>;
    runProbe: (id: string, args: ProbeRunArgs) => Promise<any>;
}

class HttpProbeClient implements IProbeApiClient {
    constructor(private baseUrl: string = "api/probe") {
    }
    public getProbes(): Promise<ProbeInfo[]> {
        return fetch(this.baseUrl)
            .then((response: Response) => {
                return response.json();
            })
            .then((probes: ProbeInfo[]) => {
                return probes;
            });
    }
    public runProbe(id: string, args: ProbeRunArgs): Promise<any> {
        return fetch(`${this.baseUrl}/${id}`, {
            method: "POST",
            mode: "cors",
            cache: "no-cache",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json",
            },
            body: JSON.stringify(args)
        })
            .then((response: Response) => {
                return response.json();
            })
            .then((result: any) => {
                return result;
            });
    }
}

class MockProbeClient implements IProbeApiClient {
    private probes: ProbeInfo[] = [
        {
            id: "Test",
            description: "This is a test",
            args: [
                { name: "arg1", type: ProbeArgType.String, isRequired: false },
            ],
        },
        {
            id: "Donkey",
            description: "This is a donkey",
            args: [
                { name: "arg2", type: ProbeArgType.Date, isRequired: true },
                { name: "arg3", type: ProbeArgType.Number, isRequired: true },
            ],
        },
    ];

    public async getProbes(): Promise<ProbeInfo[]> {
        return Promise.resolve(this.probes);
    }

    public async runProbe(id: string, args: ProbeRunArgs): Promise<any> {
        const probe: ProbeInfo = this.probes.find(p => p.id === id);
        if (probe != null) {
            return Promise.resolve({ probe, args });
        } else {
            return Promise.reject(`Probe ${id} does not exist`);
        }
    }
}

const config: ProbeConfig = { useMockData: false };
const client: IProbeApiClient = config.useMockData ? new MockProbeClient() : new HttpProbeClient();
export default client;