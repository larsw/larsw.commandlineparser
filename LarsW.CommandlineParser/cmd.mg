module LarsW.Languages
{
    language CommandLineLang
    {
        syntax Main = p:Parameters => p;
        syntax Parameters = p:Parameter => Parameters { p }
                          | list:Parameters p:Parameter => Parameters {valuesof(list), p};
                          
        syntax Parameter = "/" s:Switch p:Payload => Parameter{Name {s}, Payload {valuesof(p)}};
        syntax Payload = p:(SimplePayload | ComplexPayload)? => p;
        token ComplexPayload = '"' p:(^'"')* '"' => p;
        
        token SimplePayload = ("a".."z"|"A".."Z"|"0".."9")+;
        token Switch = ("a".."z"|"A".."Z")+;
        interleave Whitespace = " "|"\t";
    }
}