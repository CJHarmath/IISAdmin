function wget($url)
{
	$webClient = new-object net.webclient
	register-objectEvent $webClient downloadStringCompleted e1 {write-host $args[1].result}  #positional input params
	$webClient.downloadStringAsync($url)
}


80..1079 | % { $url = "http://localhost:$_/"; write-host $url; wget $url }