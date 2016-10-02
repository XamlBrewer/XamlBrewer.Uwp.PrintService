using System.Collections.Generic;
using XamlBrewer.Uwp.PrintService.Models;

namespace XamlBrewer.Uwp.PrintService.DataAccessLayer
{
    // All texts come from
    // https://sites.google.com/site/italiancommedia
    public static class Dal
    {
        public static List<TypeModel> GetTypes()
        {
            var result = new List<TypeModel>();

            result.Add(new TypeModel()
            {
                Name = "Innamorati",
                Description = "The Innamorati are the romantic leads -- 'Lovers' would be an accurate translation for their title. The male is the inamorato and the female is the inamorata. In traditional commedia, the romance of the innamorati is the driving plot of the show -- it may be a story of lovers kept separated by circumstance, or it could be a tale of the vecchio and the inamorato competing for the affections of the inamorata, but the love story was the heart of the show and the event that sent the other characters into action."
            });

            result.Add(new TypeModel()
            {
                Name = "Zanni",
                Description = "The Zanni are often referred to as 'servant' characters, but this is not entirely correct, and almost all zanni have traditional uses for filling out other types of careers ranging from shopkeepers to politicians. However, they all are most commonly shown to be servants of the vecchi or innamorati, and so the association is not unfounded."
            });

            result.Add(new TypeModel()
            {
                Name = "Vecchi",
                Description = "The word vecchi literally means 'old men'. In fact they are usually the authority figures in the story -- 'The Man' who is holding back the little people -- with the parts most typically fulfilled by the Doctor and Pantalone, although other characters like the Captain and Ruffiana can fall into this position as well. Storywise their usual purpose is to act as the villains and opponents."
            });

            return result;
        }

        public static List<CharacterModel> GetCharacters()
        {
            var result = new List<CharacterModel>();

            result.Add(new CharacterModel()
            {
                Name = "Brighella; or Brigella, Brighelle.",
                Description = "An ill-tempered but intelligent zanni, he is known to be dangerous and may even commit murders or other violent crimes. He is selfish and opportunistic -- as is the case with many of the stock characters -- but unlike the other zanni who are often stupid or at least gullible, Brighella is cunning and can manipulate circumstances in ways that would be beyond the other characters; any failure of his schemings will usually be due to bad luck on his part, rather than any real problem with his plan. He traditionally shows no real sense of honor, and will rob his dearest friend if he finds the chance; and only demonstrates loyalty to others if he discovers it to be to his own best advantage. His name comes from an old Italian word that means 'brawl' and so in English his name could be rendered as Brawley. His costume was usually white with green trimmings, and his mask an olive-color or (less-commonly) brown, with a hooked nose. Duchartre describes his traditional hat as a toque with a green border, though most illustrations seem to show Brighella with a peasant's bonnet sometimes called a 'muffin hat', similar to what most other zanni commonly wear. A modern Brighella-type character can be found in Edmund Blackadder of the Blackadder series.",
                PrimaryComicTrait = "Malicious intelligence, manifesting as insults, trouble-making, schemes, and brawls.",
                ImagePath = "Assets/Characters/brighella.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Burrattino; or Burattino or Burratino.",
                Description = "One of the zanni, with an extremely good nature -- obviously trustworthy enough that in Fortunata Isabella, the inamorata chooses him as her sole companion on a cross-country trip. He's not usually shown to be particularly smart, and he, like many zanni, is often inclined toward gluttony and lust. He is easily brought to tears by any kind of bad news (such as discovering he's eaten all the macaroni) and can lament these things at length. Illustrations show his costume to be a slightly baggy shirt and pants, decorated with small bows or ribbons, and wearing a collar. His hat is a kind of flat-cap with a narrow brim. The name Burrattino means little-donkey. A modern version of Burrattino can be found in the character of Burton 'Gus' Guster on the show Psych.",
                PrimaryComicTrait = "Soft-hearted to excess.",
                ImagePath = "Assets/Droids/"
            });

            result.Add(new CharacterModel()
            {
                Name = "Captain; or Capitano.",
                Description = "Unusually, the character of the Captain can fall into both the zanni and the vecchi categories, and can even fill the part of the inamorato on occasion. He is opportunistic and greedy, and in many scenari he is revealed to have never been a captain at all; and if he does have legitimate claim to the title he only earned it through deception and bravado. The other characters may or may not be fooled by his claims, depending on the needs of the story. He usually wears a fancy version of a period military uniform, and may or may not have a mask. If he is masked, it is usually flesh-colored with a long nose and mustache that turns up at the corners. He also is frequently portrayed as wearing glasses -- in past times these would have been a fashion accessory, akin to sunglasses today. He is usually played as being an extreme coward behind his bravado, though once in a while the character is known to demonstrate true courage; nevertheless, even when he does, it is so ineptly applied that his action is still a miserable failure. A modern example of a character in the style of the Captain is the title role of the show Invader Zim, an arrogant and exceedingly self-centered alien who earned a high military ranking simply because his superiors wanted to put him “in charge” of the most distant planet they could think of, so they'd never have to see him again.",
                PrimaryComicTrait = "Egotism and megalomania.",
                ImagePath = "Assets/Characters/capitano.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Cietrulo; or Cetrulllo.",
                Description = "Not a well-documented character, he turns up in the early 17th century Feather-Book of Dionisio Minaggio. The illustration shows the character to bear a strong resemblance to Scaramouch, and so is likely a variant on either him or on the Captain, and this is further supported in that the illustration appears to portray him threatening to pull his sword on another zanni; but Allardyce Nicoll in his book The World of Harlequin speculates he might be a variant on the character Coviello. The name Cietrulo is clearly meant to be a play on the Italian word citrulo, which means idiot, and gives some more insight as to the character's personality. He is apparently an unmasked character, but with such a name is definitely a zanni or a vecchio, rather than an inamorato.",
                PrimaryComicTrait = "Probably a short temper; see also Captain, Scaramouch, and Coviello.",
                ImagePath = "Assets/Droids/"
            });

            result.Add(new CharacterModel()
            {
                Name = "Cola; or Colafronio.",
                Description = "Usually a zanni, but sometimes classed as a vecchio and occasionally even an inamorato; he is depicted as well-dressed and wearing glasses, suggesting he might be a variant on the Captain, who has similar versatility. See also: Pasquariello.",
                PrimaryComicTrait = "",
                ImagePath = "Assets/Droids/"
            });

            result.Add(new CharacterModel()
            {
                Name = "Columbine, Colombine; or Columbina.",
                Description = "First appears in the seventeenth century, as a variation of the soubrette or maid, who were the female counterparts of the zanni. Columbine is usually amorous and is often in love with someone or another in any scenario, most typically Harlequin. She may be variously prim and dainty, or she may be bold and uncouth; but she's never as crude or vulgar as Francesquina or Ruffiana due to her intelligence and legitimate affection for others. Modern commedie often use her to provide a voice of reason in the show and utilize her more as a straight-man for the other characters. As is usual for the female roles, her costume tends to vary by the fashions of the day, and at times can be as elaborate as that of the inamorata, and at other points may be ragged and drab. Her outfit became somewhat standardized in the nineteenth century as a gown, usually white in color, frozen in the fashion of the previous century. She frequently wears no mask at all, though sometimes she may have a small eye-mask. A modern character somewhat in the style of Columbina can be found in Babs Bunny from the Tiny Toons cartoon series, in the way she often has small romantic troubles and indulges in girly activities. Yung Hee on Mike Tyson Mysteries depicts a Columbina in a straight man role.",
                PrimaryComicTrait = "The antics of love/lust. She can also be a straight man, meaning she has no primary comic trait and instead serves mainly to aid jokes from other characters.",
                ImagePath = "Assets/Characters/colombina.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Coviello; or Covielle.",
                Description = "His name is a double-diminutive of the name Giacomo. Callot's illustrations show his mask to possess an extremely long nose, protruding as far as the elbow of his outstretched arm, and he usually wears a plumed hat or headdress as part of his costume. Colors of his mask can vary but seem to usually have red in the cheek area. He falls into the category of the zanni — though he appears to have some common lineage with the Captain — and has a stronger popularity in earlier plays. The character was well-known enough in Italy that 'coviello' became a term for a boastful idiot. His actual character can be variable, and many traditional scenarios and plays portray him as quite smart. His primary qualities are conceit and bluntness — he never fears to call a spade a spade.",
                PrimaryComicTrait = "Bluntness.",
                ImagePath = "Assets/Characters/coviello.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Doctor.",
                Description = "Common names for the Doctor are Graziano Baloardo and Spaccastrummolo, which roughly translate in English, respectively, as Dr. Gratian Stoupide and Dr. Hack-and-Bandage. According to Duchartre, the character first appears during the sixteenth century, he is summarized as having “spent his whole life learning everything without understanding anything.” The Doctor is one of the vecchi, and is therefore by demand of the type prone to commit all seven of the deadly sins. He has an unusual mask that covers only the nose and forehead, either black or flesh-tone, and dresses in black. Early doctors wore caricatures of the medical robes of their era, but in the mid-seventeenth century the costume was modified to a jacket of Louis XIV style, extremely wide hat, breeches, and a ruff collar. He is rarely shown as being even remotely competent in his profession, and common sources of humor stem from his low cure rates and the bizarre (and obviously useless) treatments he administers. A modern version of this character can be found in the form of Professor Farnsworth on Futurama — an extremely elderly, amoral, senile and deranged scientist who appears to spend most of his time inventing useless and ridiculous devices and ordering around his employees.",
                PrimaryComicTrait = "Egotism and pride; often conflicting with his actual stupidity and ineptitude.",
                ImagePath = "Assets/Characters/dottore.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Franceschina; or Francesquina.",
                Description = "A soubrette, with a particularly libidinous nature. She's a bit too skanky to come off as truly sexy, but she's good enough for the likes of most zanni and vecchi. If she's unmarried, she doesn't care; and if she's married, she still doesn't care. A series of illustrations from the Recueil Fossard show her with a rather Rubenesque figure, gushing out of her corset and with the spiral lacing on her dress ready to burst. Her costume is of a low-class servant.",
                PrimaryComicTrait = "Slut.",
                ImagePath = "Assets/Droids/"
            });

            result.Add(new CharacterModel()
            {
                Name = "Geronte; or Gerontes.",
                Description = "A French version of Pantalone. Though still old, he is usually portrayed as less mean or miserly and instead more ignorant or naive. 19th century costume designs show him as an old man dressed as a gentleman of the late 17th or early 18th century, but with flamboyantly bright red rolled stockings and a variety of unfashionable hats. He does not appear to wear a mask. His name is from a Greek word meaning elder or old man. ",
                PrimaryComicTrait = "Ignorance/stupidity.",
                ImagePath = "Assets/Droids/"
            });

            result.Add(new CharacterModel()
            {
                Name = "Harlequin; or Arlequin or Arlecchino.",
                Description = "Perhaps the most popular and definitely best-known of the commedia characters. There are many dubious etymologies of his name, often linking him to mythical beings or spirits, but no one can say for sure whence the word originates. My own best guess, based on information available to me, is that it comes from Frankish karalchin, or 'little man' (cognate to the names Karl and Charles.) Harlequin's early costume was a kind of unitard or jumpsuit decorated with patches, meant to indicate a garment so ragged it was more patches than real material. Over time it evolved into the diamond or triangle pattern that has come to distinguish him. Later versions show him in a two-piece outfit made from a shirt and pants. Interestingly, his outfit has always been belted around the hips, instead of at the waist. Harlequin is traditionally portrayed by a physically agile actor and makes use of slapstick and stunts. His character is often not particularly bright though the extremes to which this is taken vary by the scenario. He wears a dark brown or black colored mask, sometimes with a beard or mustache attached. Old style Harlequins often wore a hat made from a dead animal, though from the eighteenth century on a bicorn or tricorn hat has become traditional. Another attribute of Harlequin is a wooden prop -- usually a wooden sword or a wooden stick (originally a slapstick, but later evolutions show it as just a walking stick or cane.) ",
                PrimaryComicTrait = "Making a nuisance of himself. ",
                ImagePath = "Assets/Characters/arlecchino.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Magnifico.",
                Description = "A variant of Pantalone, popular in the late 16th/early 17th centuries. Whereas Pantalone is usually of the merchant class, Magnifico is more likely to be an elected official or aristocrat, but on the whole there is no significant difference between the two characters.",
                PrimaryComicTrait = "His name suggests grace, generosity and command, all of which he lacks. See also: Pantalone.",
                ImagePath = "Assets/Droids/"
            });

            result.Add(new CharacterModel()
            {
                Name = "Mezzetino; or Mezzetin.",
                Description = "A variant on Brighella, he's a bit (but only a bit) less violent than the big brawler and instead is more interested in the ladies. Mezzetino's name suggests that alcohol is also another of his interests (from mezzeta meaning a wine-measure or a pint, according to John Florio's dictionary of 1611 -- though Lynne Lawner in Harlequin on the Moon claims the name actually refers to the character's role as a middle-man or 'go between.') He is smart but often seems to make a poor impression on others -- Duchartre even gives an example where he's downright creepy, flirting with a girl by explaining how he has murdered his last wife and will murder his current wife to be with her. Overall, it seems, he is a slightly less-capable Brighella. Mezzetino's costume began as a baggy white costume like that of the generic zanni, but later evolved to a kind of livery or else a tunic and breeches, usually striped. Watteau shows Mezzetin in pink stripes or in pink, yellow and blue pastel stripes, and Maurice Sand shows red and white stripes. Brunelleschi shows purple and white stripes. Mezzetino usually is shown wearing  cape or tabaro and a ruff or clown collar. He has a tradition of being unmasked. A bonnet or muffin hat finishes the costume. The character of Chico Marx is a Mezzetin-like character.",
                PrimaryComicTrait = "See Harlequin or Brighella.",
                ImagePath = "Assets/Characters/mezzetino.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Pantalone; or Pantaloon.",
                Description = "A vecchio, and one of the older characters of the commedia, both historically and in canon. In fact, the older the show states Pantalone's age to be, the better. He is usually portrayed as being of the merchant class though he may or may not be wealthy; if he is, it doesn't matter as he's usually so averse to spending any of his money that his lifestyle is almost that of a beggar. His costume consists of pants and a shirt or else a jumpsuit, usually red in color, with a long black coat or a cape thrown over. His mask is meant to portray an ancient old man, very wrinkled, with a large, long nose. Pantalone's traditional costume of long trousers, which was his attribute even before such garments were fashionably worn, resulted in the term “pants” “pantaloons” and so on becoming the name for such garments. His actual name may be from Greek Pantaleon, a clown mentioned by the ancient author Athenaeus, suggesting his origins to be quite old indeed. Modern Pantalone-like characters include Mr. Burns on The Simpsons (whose face is even drawn in such a way as to resemble Pantalone's mask) and Robert 'Granddad' Freeman on Boondocks.",
                PrimaryComicTrait = "Greed and stinginess -- usually of money but can apply it to women, power, food, or whatever else captures his fancy.",
                ImagePath = "Assets/Characters/pantalone.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Pasquariello, Paschiarello; or Pasquariel.",
                Description = "A Capitano variant, usually a zanni but sometimes a vecchio or inamorato. His costume is shown as a tabaro, jacket, and breeches with decorative garters. On his head appears to be a skullcap, and he looks to be unmasked. Later versions replace the tabaro with a clown-collar and add stripes to the design. Had acrobatic tendencies.",
                PrimaryComicTrait = "",
                ImagePath = "Assets/Droids/"
            });

            result.Add(new CharacterModel()
            {
                Name = "Pierrot; Pedrolino; Peterkin.",
                Description = "Ranks with Harlequin among the most popular characters of the commedia. He particularly took off in English-speaking countries after the character was revamped during the late seventeenth century to be more innocent and romantic. An indication of his popularity comes in the works of Wodehouse, who declares that for costume parties, every well-bred Englishman dresses as Pierrot. This costume consists of a white, baggy jumpsuit, or else loose trousers and a button-down shirt, sometimes with overly-long sleeves. A ruff or a clown collar around the neck is almost always worn, and the actor leaves the face unmasked and made-up with white powder. A skullcap is worn on the head to hide the hair, sometimes topped with another wider-brimmed hat. Occasionally he can be found wearing a bonnet or a tall toque. A Pierrot-like character can be found in the roles favored by Buster Keaton during his heyday (Bertie in The Saphead, Johnnie in The General, etc.)",
                PrimaryComicTrait = "Originally tended to play pranks and insult other characters for humor; later came to be known for innocence and dreaminess.",
                ImagePath = "Assets/Characters/pagliacchio.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Purricinella; or Pulcinella, Polichinelle, Punchinello.",
                Description = "Pot-bellied and hunchbacked zanni, his shape is somewhat like that of a chicken, and this is probably the origin of his name (from medieval Italian pollicino, a young pullet or chicken.) His mask is formed to have a long, hooked, beak-like nose, and his costume usually includes a tall hat of some form. His actual garments are similar to Pierrot's. Pulcinella's primary trait is a tendency toward malice and selfishness which is usually covered by an ignorance — or pretense thereof — as to the harm he's causing. Like Brighella, he's willing to commit murder, but Pulcinella will often find a way to make it seem like an accident or even to trick or confuse the victim into killing himself. He evolved in English-speaking countries into Mr. Punch of the notorious Punch and Judy shows. The character of Betelgeuse (Beetlejuice) as played by Michael Keaton is a sort of modern Pulicinella character.",
                PrimaryComicTrait = "Gleeful malice.",
                ImagePath = "Assets/Characters/pulcinella.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Ruffiana; or La Ruffiana.",
                Description = "A female vecchio, not well documented but recently grown in popularity due to the new availability of female actresses and the need to fit them with roles. Her name, according to Florio, means 'a woman bawde' i.e. a whore or a madam.  She is often played as gossipy, to the point that sometimes her name gets mistranslated as 'gossiper.' In any case, her character is that of a low class woman, even if the story has set her up to be financially well-off through her occupations or marriages. She doesn't appear to have a standard costume but evidently would dress as flamboyantly as her financial condition would allow. Oddly for a female character, it appears she did traditionally wear a mask, which looks to have been modeled after the bauta style, but with a higher mouth so as not to obscure her speech. Lady Booby from the film adaption of Joseph Andrews is a Ruffiana-like character.",
                PrimaryComicTrait = "Old whore. See also: Francesquina.",
                ImagePath = "Assets/Characters/ruffiana.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Scapino; or Scappino, Scapin.",
                Description = "A Brighella derivative; his name is related to the English word 'escape' in reference to his tendency to flee from fights, even those he himself began. Scapino tends to make a confusion of anything he undertakes and metaphorically 'flees' from one thought, activity or love interest to another, as his name implies, although he usually will return to it -- eventually. Self-preservation and self-interest are his main concerns. This is not to say his wits are without merit. In The Impostures of Scapin, Zerbinette mentions what “a clever servant [Léandre] has. His name is Scapin. He is a most wonderful man and deserves the highest praise.” He is a schemer and scoundrel, and takes a certain pride in these facts. He was originally a masked character, although later versions usually have the actor simply powder his face. He is traditionally shown with a hooked nose and a pointed beard. Later versions show his costume with green (or sometimes turquoise) and white stripes, similar to Mezzetino's red and white, but Callot shows Scapino in an outfit similar to the early Brighella's, white with a tabaro and a sword on his belt, and topped with a torn hat adorned with feathers.",
                PrimaryComicTrait = "Malicious intelligence combined with physical agility.",
                ImagePath = "Assets/Characters/scapino.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Scaramouche; or Scaramouch or Scaramuccia.",
                Description = "According to Duchartre, Scaramouche is a variation of the Captain. Until the mid-seventeenth century he was a masked character, but later became a role in which the actor merely powders his face, if that. It is traditional for his character to dress in all or mostly black, with a bonnet and a white ruff or clown collar, and often with a tabaro. His personality is similar to the Captain, though a little more mellow on the braggadocio. A modern variation of this character can be found in Daffy Duck.",
                PrimaryComicTrait = "Arrogance.",
                ImagePath = "Assets/Characters/scaramouche.jpg"
            });

            result.Add(new CharacterModel()
            {
                Name = "Tartaglia.",
                Description = "The word tartaglia means a stutterer or stammerer, and this is the primary trait of the character. For the sake of humor he often will find himself stuck on the most obscene syllable in any given word. Tartaglia frequently is an official of some kind, like a judge or a minister to the king. He usually is in the character class of a vecchio or a zanni. He does not appear to have a standard costume, but Maurice Sand shows him in a green and yellow striped clown outfit. He often is shown with thick glasses and is meant to be old, so his mask uses these qualities. A rounded nose is also common. A modernized Tartaglia can be found in the character of Porky Pig.",
                PrimaryComicTrait = "Stutter.",
                ImagePath = "Assets/Characters/tartaglia.jpg"
            });

            return result;
        }
    }
}
