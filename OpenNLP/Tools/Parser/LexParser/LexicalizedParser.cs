using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Parser.LexParser
{
    [Serializable]
    public class LexicalizedParser /*: ParserGrammar*/
    {
        public Lexicon lex;
          public BinaryGrammar bg;
          public UnaryGrammar ug;
          public DependencyGrammar dg;
          public Index<String> stateIndex, wordIndex, tagIndex;

          private Options op;

          @Override
          public Options getOp() { return op; }

          public Reranker reranker = null;

          @Override
          public TreebankLangParserParams getTLPParams() { return op.tlpParams; }

          @Override
          public TreebankLanguagePack treebankLanguagePack() { return getTLPParams().treebankLanguagePack(); }

          @Override
          public String[] defaultCoreNLPFlags() {
            return getTLPParams().defaultCoreNLPFlags();
          }

          @Override
          public boolean requiresTags() {
            return false;
          }

          private static final String SERIALIZED_PARSER_PROPERTY = "edu.stanford.nlp.SerializedLexicalizedParser";
          public static final String DEFAULT_PARSER_LOC = ((System.getenv("NLP_PARSER") != null) ?
                                                           System.getenv("NLP_PARSER") :
                                                           "edu/stanford/nlp/models/lexparser/englishPCFG.ser.gz");

          /**
           * Construct a new LexicalizedParser object from a previously
           * serialized grammar read from a System property
           * <code>edu.stanford.nlp.SerializedLexicalizedParser</code>, or a
           * default classpath location
           * ({@code edu/stanford/nlp/models/lexparser/englishPCFG.ser.gz}).
           */
          /*public static LexicalizedParser loadModel() {
            return loadModel(new Options());
          }*/

          /**
           * Construct a new LexicalizedParser object from a previously
           * serialized grammar read from a System property
           * <code>edu.stanford.nlp.SerializedLexicalizedParser</code>, or a
           * default classpath location
           * ({@code edu/stanford/nlp/models/lexparser/englishPCFG.ser.gz}).
           *
           * @param op Options to the parser.  These get overwritten by the
           *           Options read from the serialized parser; I think the only
           *           thing determined by them is the encoding of the grammar
           *           iff it is a text grammar
           */
          /*public static LexicalizedParser loadModel(Options op,
                                                    String ... extraFlags) {
            String source = System.getProperty(SERIALIZED_PARSER_PROPERTY);
            if (source == null) {
              source = DEFAULT_PARSER_LOC;
            }
            return loadModel(source, op, extraFlags);
          }*/

          /*public static LexicalizedParser loadModel(String parserFileOrUrl,
                                                    String ... extraFlags) {
            return loadModel(parserFileOrUrl, new Options(), extraFlags);
          }*/

          /*public static LexicalizedParser loadModel(String parserFileOrUrl,
                                                    List<String> extraFlags) {
            return loadModel(parserFileOrUrl, new Options(), extraFlags);
          }*/

          /**
           * Construct a new LexicalizedParser.  This loads a grammar
           * that was previously assembled and stored as a serialized file.
           * @param parserFileOrUrl Filename/URL to load parser from
           * @param op Options for this parser. These will normally be overwritten
           *     by options stored in the file
           * @throws IllegalArgumentException If parser data cannot be loaded
           */
          public static LexicalizedParser loadModel(String parserFileOrUrl, /*Options op,*/
                                                    List<string> extraFlags) {
            //    System.err.print("Loading parser from file " + parserFileOrUrl);
            LexicalizedParser parser = getParserFromFile(parserFileOrUrl, op);
            if (extraFlags.Count > 0) {
              parser.setOptionFlags(extraFlags);
            }
            return parser;
          }

          /**
           * Reads one object from the given ObjectInputStream, which is
           * assumed to be a LexicalizedParser.  Throws a ClassCastException
           * if this is not true.  The stream is not closed.
           */
          public static LexicalizedParser loadModel(ObjectInputStream ois) {
            try {
              Object o = ois.readObject();
              if (o instanceof LexicalizedParser) {
                return (LexicalizedParser) o;
              }
              throw new ClassCastException("Wanted LexicalizedParser, got " +
                                           o.getClass());
            } catch (IOException e) {
              throw new RuntimeIOException(e);
            } catch (ClassNotFoundException e) {
              throw new RuntimeException(e);
            }
          }

          public static LexicalizedParser loadModelFromZip(String zipFilename,
                                                           String modelName) {
            LexicalizedParser parser = null;
            try {
              File file = new File(zipFilename);
              if (file.exists()) {
                ZipFile zin = new ZipFile(file);
                ZipEntry zentry = zin.getEntry(modelName);
                if (zentry != null) {
                  InputStream in = zin.getInputStream(zentry);
                  // gunzip it if necessary
                  if (modelName.endsWith(".gz")) {
                    in = new GZIPInputStream(in);
                  }
                  ObjectInputStream ois = new ObjectInputStream(in);
                  parser = loadModel(ois);
                  ois.close();
                  in.close();
                }
                zin.close();
              } else {
                throw new FileNotFoundException("Could not find " + modelName +
                                                " inside " + zipFilename);
              }
            } catch (IOException e) {
              throw new RuntimeIOException(e);
            }
            return parser;
          }

          public static LexicalizedParser copyLexicalizedParser(LexicalizedParser parser) {
            return new LexicalizedParser(parser.lex, parser.bg, parser.ug, parser.dg, parser.stateIndex, parser.wordIndex, parser.tagIndex, parser.op);
          }

          public LexicalizedParser(Lexicon lex, BinaryGrammar bg, UnaryGrammar ug, DependencyGrammar dg, Index<String> stateIndex, Index<String> wordIndex, Index<String> tagIndex, Options op) {
            this.lex = lex;
            this.bg = bg;
            this.ug = ug;
            this.dg = dg;
            this.stateIndex = stateIndex;
            this.wordIndex = wordIndex;
            this.tagIndex = tagIndex;
            this.op = op;
          }


          /**
           * Construct a new LexicalizedParser.
           *
           * @param trainTreebank a treebank to train from
           */
          public static LexicalizedParser trainFromTreebank(Treebank trainTreebank,
                                                            GrammarCompactor compactor,
                                                            Options op) {
            return getParserFromTreebank(trainTreebank, null, 1.0, compactor, op, null, null);
          }

          public static LexicalizedParser trainFromTreebank(String treebankPath,
                                                            FileFilter filt,
                                                            Options op) {
            return trainFromTreebank(makeTreebank(treebankPath, op, filt), op);
          }

          public static LexicalizedParser trainFromTreebank(Treebank trainTreebank,
                                                            Options op) {
            return trainFromTreebank(trainTreebank, null, op);
          }


          /**
           * Will process a list of strings into a list of HasWord and return
           * the parse tree associated with that list.
           */
          public Tree parseStrings(List<String> lst) {
            List<Word> words = new ArrayList<Word>();
            for (String word : lst) {
              words.add(new Word(word));
            }
            return parse(words);
          }

          /**
           * Parses the list of HasWord.  If the parse fails for some reason,
           * an X tree is returned instead of barfing.
           */
          public Tree parse(List<? extends HasWord> lst) {
            try {
              ParserQuery pq = parserQuery();
              if (pq.parse(lst)) {
                Tree bestparse = pq.getBestParse();
                // -10000 denotes unknown words
                bestparse.setScore(pq.getPCFGScore() % -10000.0);
                return bestparse;
              }
            } catch (Exception e) {
              System.err.println("Following exception caught during parsing:");
              e.printStackTrace();
              System.err.println("Recovering using fall through strategy: will construct an (X ...) tree.");
            }
            // if can't parse or exception, fall through
            return ParserUtils.xTree(lst);
          }

          public List<Tree> parseMultiple(final List<? extends List<? extends HasWord>> sentences) {
            List<Tree> trees = new ArrayList<Tree>();
            for (List<? extends HasWord> sentence : sentences) {
              trees.add(parse(sentence));
            }
            return trees;
          }

          /**
           * Will launch multiple threads which calls <code>parse</code> on
           * each of the <code>sentences</code> in order, returning the
           * resulting parse trees in the same order.
           */
          public List<Tree> parseMultiple(final List<? extends List<? extends HasWord>> sentences, final int nthreads) {
            MulticoreWrapper<List<? extends HasWord>, Tree> wrapper = new MulticoreWrapper<List<? extends HasWord>, Tree>(nthreads, new ThreadsafeProcessor<List<? extends HasWord>, Tree>() {
                public Tree process(List<? extends HasWord> sentence) {
                  return parse(sentence);
                }
                public ThreadsafeProcessor<List<? extends HasWord>, Tree> newInstance() {
                  return this;
                }
              });
            List<Tree> trees = new ArrayList<Tree>();
            for (List<? extends HasWord> sentence : sentences) {
              wrapper.put(sentence);
              while (wrapper.peek()) {
                trees.add(wrapper.poll());
              }
            }
            wrapper.join();
            while (wrapper.peek()) {
              trees.add(wrapper.poll());
            }
            return trees;
          }

          /** Return a TreePrint for formatting parsed output trees.
           *  @return A TreePrint for formatting parsed output trees.
           */
          public TreePrint getTreePrint() {
            return op.testOptions.treePrint(op.tlpParams);
          }

          /**
           * Similar to parse(), but instead of returning an X tree on failure, returns null.
           */
          public Tree parseTree(List<? extends HasWord> sentence) {
            ParserQuery pq = parserQuery();
            if (pq.parse(sentence)) {
              return pq.getBestParse();
            } else {
              return null;
            }
          }

          public List<Eval> getExtraEvals() {
            if (reranker != null) {
              return reranker.getEvals();
            } else {
              return Collections.emptyList();
            }
          }


          public List<ParserQueryEval> getParserQueryEvals() {
            return Collections.emptyList();
          }


          @Override
          public ParserQuery parserQuery() {
            if (reranker == null) {
              return new LexicalizedParserQuery(this);
            } else {
              return new RerankingParserQuery(op, new LexicalizedParserQuery(this), reranker);
            }
          }

          public LexicalizedParserQuery lexicalizedParserQuery() {
            return new LexicalizedParserQuery(this);
          }

          public static LexicalizedParser getParserFromFile(String parserFileOrUrl/*, Options op*/) {
            LexicalizedParser pd = getParserFromSerializedFile(parserFileOrUrl);
            if (pd == null) {
              pd = getParserFromTextFile(parserFileOrUrl/*, op*/);
            }
            return pd;
          }

          private static Treebank makeTreebank(String treebankPath, Options op, FileFilter filt) {
            System.err.println("Training a parser from treebank dir: " + treebankPath);
            Treebank trainTreebank = op.tlpParams.diskTreebank();
            System.err.print("Reading trees...");
            if (filt == null) {
              trainTreebank.loadPath(treebankPath);
            } else {
              trainTreebank.loadPath(treebankPath, filt);
            }

            Timing.tick("done [read " + trainTreebank.size() + " trees].");
            return trainTreebank;
          }

          private static DiskTreebank makeSecondaryTreebank(String treebankPath, Options op, FileFilter filt) {
            System.err.println("Additionally training using secondary disk treebank: " + treebankPath + ' ' + filt);
            DiskTreebank trainTreebank = op.tlpParams.diskTreebank();
            System.err.print("Reading trees...");
            if (filt == null) {
              trainTreebank.loadPath(treebankPath);
            } else {
              trainTreebank.loadPath(treebankPath, filt);
            }
            Timing.tick("done [read " + trainTreebank.size() + " trees].");
            return trainTreebank;
          }

          public Lexicon getLexicon() {
            return lex;
          }

          /**
           * Saves the parser defined by pd to the given filename.
           * If there is an error, a RuntimeIOException is thrown.
           */
          public void saveParserToSerialized(String filename) {
            try {
              System.err.print("Writing parser in serialized format to file " + filename + ' ');
              ObjectOutputStream out = IOUtils.writeStreamFromString(filename);
              out.writeObject(this);
              out.close();
              System.err.println("done.");
            } catch (IOException ioe) {
              throw new RuntimeIOException(ioe);
            }
          }

          /**
           * Saves the parser defined by pd to the given filename.
           * If there is an error, a RuntimeIOException is thrown.
           */
          public void saveParserToTextFile(String filename) {
            if (reranker != null) {
              throw new UnsupportedOperationException("Sorry, but parsers with rerankers cannot be saved to text file");
            }
            try {
              System.err.print("Writing parser in text grammar format to file " + filename);
              OutputStream os;
              if (filename.endsWith(".gz")) {
                // it's faster to do the buffering _outside_ the gzipping as here
                os = new BufferedOutputStream(new GZIPOutputStream(new FileOutputStream(filename)));
              } else {
                os = new BufferedOutputStream(new FileOutputStream(filename));
              }
              PrintWriter out = new PrintWriter(os);
              String prefix = "BEGIN ";

              out.println(prefix + "OPTIONS");
              op.writeData(out);
              out.println();
              System.err.print(".");

              out.println(prefix + "STATE_INDEX");
              stateIndex.saveToWriter(out);
              out.println();
              System.err.print(".");

              out.println(prefix + "WORD_INDEX");
              wordIndex.saveToWriter(out);
              out.println();
              System.err.print(".");

              out.println(prefix + "TAG_INDEX");
              tagIndex.saveToWriter(out);
              out.println();
              System.err.print(".");

              String uwmClazz = ((lex.getUnknownWordModel() == null) ? "null" :
                           lex.getUnknownWordModel().getClass().getCanonicalName());
              out.println(prefix + "LEXICON " + uwmClazz);
              lex.writeData(out);
              out.println();
              System.err.print(".");

              out.println(prefix + "UNARY_GRAMMAR");
              ug.writeData(out);
              out.println();
              System.err.print(".");

              out.println(prefix + "BINARY_GRAMMAR");
              bg.writeData(out);
              out.println();
              System.err.print(".");

              out.println(prefix + "DEPENDENCY_GRAMMAR");
              if (dg != null) {
                dg.writeData(out);
              }
              out.println();
              System.err.print(".");

              out.flush();
              out.close();
              System.err.println("done.");
            } catch (IOException e) {
              System.err.println("Trouble saving parser data to ASCII format.");
              throw new RuntimeIOException(e);
            }
          }

          private static void confirmBeginBlock(String file, String line) {
            if (line == null) {
              throw new RuntimeException(file + ": expecting BEGIN block; got end of file.");
            } else if (! line.startsWith("BEGIN")) {
              throw new RuntimeException(file + ": expecting BEGIN block; got " + line);
            }
          }

          protected static LexicalizedParser getParserFromTextFile(String textFileOrUrl/*, Options op*/) {
            try {
              /*Timing tim = new Timing();
              System.err.print("Loading parser from text file " + textFileOrUrl + ' ');*/
              var reader = IOUtils.readerFromString(textFileOrUrl);
              //Timing.startTime();

              String line = reader.ReadLine();
              confirmBeginBlock(textFileOrUrl, line);
              //op.readData(reader);
              //System.err.print(".");

              line = reader.ReadLine();
              confirmBeginBlock(textFileOrUrl, line);
              Index<String> stateIndex = HashIndex.loadFromReader(reader);
              //System.err.print(".");

              line = reader.ReadLine();
              confirmBeginBlock(textFileOrUrl, line);
              Index<String> wordIndex = HashIndex.loadFromReader(reader);
              //System.err.print(".");

              line = reader.ReadLine();
              confirmBeginBlock(textFileOrUrl, line);
              Index<String> tagIndex = HashIndex.loadFromReader(in);
              //System.err.print(".");

              line = reader.ReadLine();
              confirmBeginBlock(textFileOrUrl, line);
              Lexicon lex = op.tlpParams.lex(op, wordIndex, tagIndex);
              String uwmClazz = line.split(" +")[2];
              if (!uwmClazz.equals("null")) {
                UnknownWordModel model = ReflectionLoading.loadByReflection(uwmClazz, op, lex, wordIndex, tagIndex);
                lex.setUnknownWordModel(model);
              }
              lex.readData(reader);
              //System.err.print(".");

              line = reader.readLine();
              confirmBeginBlock(textFileOrUrl, line);
              UnaryGrammar ug = new UnaryGrammar(stateIndex);
              ug.readData(reader);
              //System.err.print(".");

              line = reader.readLine();
              confirmBeginBlock(textFileOrUrl, line);
              BinaryGrammar bg = new BinaryGrammar(stateIndex);
              bg.readData(reader);
              //System.err.print(".");

              line = reader.readLine();
              confirmBeginBlock(textFileOrUrl, line);
              DependencyGrammar dg = new MLEDependencyGrammar(op.tlpParams, op.directional, op.distance, op.coarseDistance, op.trainOptions.basicCategoryTagsInDependencyGrammar, op, wordIndex, tagIndex);
              dg.readData(reader);
              //System.err.print(".");

              reader.Close();
              //System.err.println(" done [" + tim.toSecondsString() + " sec].");
              return new LexicalizedParser(lex, bg, ug, dg, stateIndex, wordIndex, tagIndex, op);
            } catch (IOException e) {
              //e.printStackTrace();
            }
            return null;
          }


          public static LexicalizedParser getParserFromSerializedFile(String serializedFileOrUrl) {
            try {
              Timing tim = new Timing();
              System.err.print("Loading parser from serialized file " + serializedFileOrUrl + " ...");
              ObjectInputStream in = IOUtils.readStreamFromString(serializedFileOrUrl);
              LexicalizedParser pd = loadModel(in);

              in.close();
              System.err.println(" done [" + tim.toSecondsString() + " sec].");
              return pd;
            } catch (InvalidClassException ice) {
              // For this, it's not a good idea to continue and try it as a text file!
              System.err.println();   // as in middle of line from above message
              throw new RuntimeException("Invalid class in file: " + serializedFileOrUrl, ice);
            } catch (FileNotFoundException fnfe) {
              // For this, it's not a good idea to continue and try it as a text file!
              System.err.println();   // as in middle of line from above message
              throw new RuntimeException("File not found: " + serializedFileOrUrl, fnfe);
            } catch (StreamCorruptedException sce) {
              // suppress error message, on the assumption that we've really got
              // a text grammar, and that'll be tried next
              System.err.println();
            } catch (Exception e) {
              System.err.println();   // as in middle of line from above message
              e.printStackTrace();
            }
            return null;
          }


          private static void printOptions(boolean train, Options op) {
            op.display();
            if (train) {
              op.trainOptions.display();
            } else {
              op.testOptions.display();
            }
            op.tlpParams.display();
          }

          public static TreeAnnotatorAndBinarizer buildTrainBinarizer(Options op) {
            TreebankLangParserParams tlpParams = op.tlpParams;
            if (!op.trainOptions.leftToRight) {
              return new TreeAnnotatorAndBinarizer(tlpParams, op.forceCNF, !op.trainOptions.outsideFactor(), !op.trainOptions.predictSplits, op);
            } else {
              return new TreeAnnotatorAndBinarizer(tlpParams.headFinder(), new LeftHeadFinder(), tlpParams, op.forceCNF, !op.trainOptions.outsideFactor(), !op.trainOptions.predictSplits, op);
            }
          }

          public static CompositeTreeTransformer buildTrainTransformer(Options op) {
            TreeAnnotatorAndBinarizer binarizer = buildTrainBinarizer(op);
            return buildTrainTransformer(op, binarizer);
          }

          public static CompositeTreeTransformer buildTrainTransformer(Options op, TreeAnnotatorAndBinarizer binarizer) {
            TreebankLangParserParams tlpParams = op.tlpParams;
            TreebankLanguagePack tlp = tlpParams.treebankLanguagePack();
            CompositeTreeTransformer trainTransformer =
              new CompositeTreeTransformer();
            if (op.trainOptions.preTransformer != null) {
              trainTransformer.addTransformer(op.trainOptions.preTransformer);
            }
            if (op.trainOptions.collinsPunc) {
              CollinsPuncTransformer collinsPuncTransformer =
                new CollinsPuncTransformer(tlp);
              trainTransformer.addTransformer(collinsPuncTransformer);
            }

            trainTransformer.addTransformer(binarizer);

            if (op.wordFunction != null) {
              TreeTransformer wordFunctionTransformer =
                new TreeLeafLabelTransformer(op.wordFunction);
              trainTransformer.addTransformer(wordFunctionTransformer);
            }
            return trainTransformer;
          }

          /** @return a pair of binaryTrainTreebank,binaryTuneTreebank.
           */
          public static Triple<Treebank, Treebank, Treebank> getAnnotatedBinaryTreebankFromTreebank(Treebank trainTreebank,
              Treebank secondaryTreebank,
              Treebank tuneTreebank,
              Options op) {
            // setup tree transforms
            TreebankLangParserParams tlpParams = op.tlpParams;
            TreebankLanguagePack tlp = tlpParams.treebankLanguagePack();

            if (op.testOptions.verbose) {
              PrintWriter pwErr = tlpParams.pw(System.err);
              pwErr.print("Training ");
              pwErr.println(trainTreebank.textualSummary(tlp));
              if (secondaryTreebank != null) {
                pwErr.print("Secondary training ");
                pwErr.println(secondaryTreebank.textualSummary(tlp));
              }
            }

            System.err.print("Binarizing trees...");

            TreeAnnotatorAndBinarizer binarizer = buildTrainBinarizer(op);
            CompositeTreeTransformer trainTransformer = buildTrainTransformer(op, binarizer);

            Treebank wholeTreebank;
            if (secondaryTreebank == null) {
              wholeTreebank = trainTreebank;
            } else {
              wholeTreebank = new CompositeTreebank(trainTreebank, secondaryTreebank);
            }

            if (op.trainOptions.selectiveSplit) {
              op.trainOptions.splitters = ParentAnnotationStats.getSplitCategories(wholeTreebank, op.trainOptions.tagSelectiveSplit, 0, op.trainOptions.selectiveSplitCutOff, op.trainOptions.tagSelectiveSplitCutOff, tlp);
              removeDeleteSplittersFromSplitters(tlp, op);
              if (op.testOptions.verbose) {
                List<String> list = new ArrayList<String>(op.trainOptions.splitters);
                Collections.sort(list);
                System.err.println("Parent split categories: " + list);
              }
            }

            if (op.trainOptions.selectivePostSplit) {
              // Do all the transformations once just to learn selective splits on annotated categories
              TreeTransformer myTransformer = new TreeAnnotator(tlpParams.headFinder(), tlpParams, op);
              wholeTreebank = wholeTreebank.transform(myTransformer);
              op.trainOptions.postSplitters = ParentAnnotationStats.getSplitCategories(wholeTreebank, true, 0, op.trainOptions.selectivePostSplitCutOff, op.trainOptions.tagSelectivePostSplitCutOff, tlp);
              if (op.testOptions.verbose) {
                System.err.println("Parent post annotation split categories: " + op.trainOptions.postSplitters);
              }
            }
            if (op.trainOptions.hSelSplit) {
              // We run through all the trees once just to gather counts for hSelSplit!
              int ptt = op.trainOptions.printTreeTransformations;
              op.trainOptions.printTreeTransformations = 0;
              binarizer.setDoSelectiveSplit(false);
              for (Tree tree : wholeTreebank) {
                trainTransformer.transformTree(tree);
              }
              binarizer.setDoSelectiveSplit(true);
              op.trainOptions.printTreeTransformations = ptt;
            }
            // we've done all the setup now. here's where the train treebank is transformed.
            trainTreebank = trainTreebank.transform(trainTransformer);
            if (secondaryTreebank != null) {
              secondaryTreebank = secondaryTreebank.transform(trainTransformer);
            }
            if (op.trainOptions.printAnnotatedStateCounts) {
              binarizer.printStateCounts();
            }
            if (op.trainOptions.printAnnotatedRuleCounts) {
              binarizer.printRuleCounts();
            }

            if (tuneTreebank != null) {
              tuneTreebank = tuneTreebank.transform(trainTransformer);
            }

            Timing.tick("done.");
            if (op.testOptions.verbose) {
              binarizer.dumpStats();
            }

            return new Triple<Treebank, Treebank, Treebank>(trainTreebank, secondaryTreebank, tuneTreebank);
          }

          private static void removeDeleteSplittersFromSplitters(TreebankLanguagePack tlp, Options op) {
            if (op.trainOptions.deleteSplitters != null) {
              List<String> deleted = new ArrayList<String>();
              for (String del : op.trainOptions.deleteSplitters) {
                String baseDel = tlp.basicCategory(del);
                boolean checkBasic = del.equals(baseDel);
                for (Iterator<String> it = op.trainOptions.splitters.iterator(); it.hasNext(); ) {
                  String elem = it.next();
                  String baseElem = tlp.basicCategory(elem);
                  boolean delStr = checkBasic && baseElem.equals(baseDel) || elem.equals(del);
                  if (delStr) {
                    it.remove();
                    deleted.add(elem);
                  }
                }
              }
              if (op.testOptions.verbose) {
                System.err.println("Removed from vertical splitters: " + deleted);
              }
            }
          }


          // TODO: Make below method work with arbitrarily large secondary treebank via iteration
          // TODO: Have weight implemented for training lexicon

          /**
           * A method for training from two different treebanks, the second of which is presumed
           * to be orders of magnitude larger.
           * <p/>
           * Trees are not read into memory but processed as they are read from disk.
           * <p/>
           * A weight (typically &lt;= 1) can be put on the second treebank.
           *
           * @param trainTreebank A treebank to train from
           * @param secondaryTrainTreebank Another treebank to train from
           * @param weight A weight factor to give the secondary treebank. If the weight
           *     is 0.25, each example in the secondaryTrainTreebank will be treated as
           *     1/4 of an example sentence.
           * @param compactor A class for compacting grammars. May be null.
           * @param op Options for how the grammar is built from the treebank
           * @param tuneTreebank  A treebank to tune free params on (may be null)
           * @param extraTaggedWords A list of words to add to the Lexicon
           * @return The trained LexicalizedParser
           */
          public static LexicalizedParser
          getParserFromTreebank(Treebank trainTreebank,
                                Treebank secondaryTrainTreebank,
                                double weight,
                                GrammarCompactor compactor,
                                Options op,
                                Treebank tuneTreebank,
                                List<List<TaggedWord>> extraTaggedWords)
          {
            // System.err.println("Currently " + new Date()); // now printed when command-line args are printed
            printOptions(true, op);
            Timing.startTime();

            Triple<Treebank, Treebank, Treebank> treebanks = TreeAnnotatorAndBinarizer.getAnnotatedBinaryTreebankFromTreebank(trainTreebank, secondaryTrainTreebank, tuneTreebank, op);
            Timing.tick("done.");

            Treebank trainTreebankRaw = trainTreebank;
            trainTreebank = treebanks.first();
            secondaryTrainTreebank = treebanks.second();
            tuneTreebank = treebanks.third();

            // +1 to account for the boundary symbol
            trainTreebank = new FilteringTreebank(trainTreebank, new LengthTreeFilter(op.trainOptions.trainLengthLimit + 1));
            if (secondaryTrainTreebank != null) {
              secondaryTrainTreebank = new FilteringTreebank(secondaryTrainTreebank, new LengthTreeFilter(op.trainOptions.trainLengthLimit + 1));
            }
            if (tuneTreebank != null) {
              tuneTreebank = new FilteringTreebank(tuneTreebank, new LengthTreeFilter(op.trainOptions.trainLengthLimit + 1));
            }

            Index<String> stateIndex;
            Index<String> wordIndex;
            Index<String> tagIndex;

            Pair<UnaryGrammar, BinaryGrammar> bgug;
            Lexicon lex;

            if (op.trainOptions.predictSplits) {
              SplittingGrammarExtractor extractor = new SplittingGrammarExtractor(op);
              System.err.print("Extracting PCFG...");
              // TODO: make use of the tagged text
              if (secondaryTrainTreebank == null) {
                extractor.extract(trainTreebank);
              } else {
                extractor.extract(trainTreebank, 1.0, secondaryTrainTreebank, weight);
              }
              bgug = extractor.bgug;
              lex = extractor.lex;
              stateIndex = extractor.stateIndex;
              wordIndex = extractor.wordIndex;
              tagIndex = extractor.tagIndex;
              Timing.tick("done.");
            } else {
              stateIndex = new HashIndex<String>();
              wordIndex = new HashIndex<String>();
              tagIndex = new HashIndex<String>();

              // extract grammars
              BinaryGrammarExtractor bgExtractor = new BinaryGrammarExtractor(op, stateIndex);
              // Extractor lexExtractor = new LexiconExtractor();
              //TreeExtractor uwmExtractor = new UnknownWordModelExtractor(trainTreebank.size());
              System.err.print("Extracting PCFG...");
              if (secondaryTrainTreebank == null) {
                bgug = bgExtractor.extract(trainTreebank);
              } else {
                bgug = bgExtractor.extract(trainTreebank, 1.0,
                                           secondaryTrainTreebank, weight);
              }
              Timing.tick("done.");

              System.err.print("Extracting Lexicon...");
              lex = op.tlpParams.lex(op, wordIndex, tagIndex);

              double trainSize = trainTreebank.size();
              if (secondaryTrainTreebank != null) {
                trainSize += (secondaryTrainTreebank.size() * weight);
              }
              if (extraTaggedWords != null) {
                trainSize += extraTaggedWords.size();
              }

              lex.initializeTraining(trainSize);
              // wsg2012: The raw treebank has CoreLabels, which we need for FactoredLexicon
              // training. If TreeAnnotator is updated so that it produces CoreLabels, then we can
              // remove the trainTreebankRaw.
              lex.train(trainTreebank, trainTreebankRaw);
              if (secondaryTrainTreebank != null) {
                lex.train(secondaryTrainTreebank, weight);
              }
              if (extraTaggedWords != null) {
                for (List<TaggedWord> sentence : extraTaggedWords) {
                  // TODO: specify a weight?
                  lex.trainUnannotated(sentence, 1.0);
                }
              }
              lex.finishTraining();
              Timing.tick("done.");
            }

            //TODO: wsg2011 Not sure if this should come before or after
            //grammar compaction
            if (op.trainOptions.ruleSmoothing) {
              System.err.print("Smoothing PCFG...");
              Function<Pair<UnaryGrammar,BinaryGrammar>,Pair<UnaryGrammar,BinaryGrammar>> smoother = new LinearGrammarSmoother(op.trainOptions, stateIndex, tagIndex);
              bgug = smoother.apply(bgug);
              Timing.tick("done.");
            }

            if (compactor != null) {
              System.err.print("Compacting grammar...");
              Triple<Index<String>, UnaryGrammar, BinaryGrammar> compacted = compactor.compactGrammar(bgug, stateIndex);
              stateIndex = compacted.first();
              bgug.setFirst(compacted.second());
              bgug.setSecond(compacted.third());
              Timing.tick("done.");
            }

            System.err.print("Compiling grammar...");
            BinaryGrammar bg = bgug.second;
            bg.splitRules();
            UnaryGrammar ug = bgug.first;
            ug.purgeRules();
            Timing.tick("done");

            DependencyGrammar dg = null;
            if (op.doDep) {
              System.err.print("Extracting Dependencies...");
              AbstractTreeExtractor<DependencyGrammar> dgExtractor = new MLEDependencyGrammarExtractor(op, wordIndex, tagIndex);
              if (secondaryTrainTreebank == null) {
                dg = dgExtractor.extract(trainTreebank);
              } else {
                dg = dgExtractor.extract(trainTreebank, 1.0, secondaryTrainTreebank, weight);
              }
              //System.err.print("Extracting Unknown Word Model...");
              //UnknownWordModel uwm = (UnknownWordModel)uwmExtractor.extract(trainTreebank);
              //Timing.tick("done.");
              Timing.tick("done.");
              if (tuneTreebank != null) {
                System.err.print("Tuning Dependency Model...");
                dg.setLexicon(lex); // MG2008: needed if using PwGt model
                dg.tune(tuneTreebank);
                Timing.tick("done.");
              }
            }

            System.err.println("Done training parser.");
            if (op.trainOptions.trainTreeFile!=null) {
              try {
                System.err.print("Writing out binary trees to "+ op.trainOptions.trainTreeFile+"...");
                IOUtils.writeObjectToFile(trainTreebank, op.trainOptions.trainTreeFile);
                IOUtils.writeObjectToFile(secondaryTrainTreebank, op.trainOptions.trainTreeFile);
                Timing.tick("done.");
              } catch (Exception e) {
                System.err.println("Problem writing out binary trees.");
              }
            }
            return new LexicalizedParser(lex, bg, ug, dg, stateIndex, wordIndex, tagIndex, op);
          }


          /**
           * This will set options to the parser, in a way exactly equivalent to
           * passing in the same sequence of command-line arguments.  This is a useful
           * convenience method when building a parser programmatically. The options
           * passed in should
           * be specified like command-line arguments, including with an initial
           * minus sign.
           * <p/>
           * <i>Notes:</i> This can be used to set parsing-time flags for a
           * serialized parser.  You can also still change things serialized
           * in Options, but this will probably degrade parsing performance.
           * The vast majority of command line flags can be passed to this
           * method, but you cannot pass in options that specify the treebank
           * or grammar to be loaded, the grammar to be written, trees or
           * files to be parsed or details of their encoding, nor the
           * TreebankLangParserParams (<code>-tLPP</code>) to use. The
           * TreebankLangParserParams should be set up on construction of a
           * LexicalizedParser, by constructing an Options that uses
           * the required TreebankLangParserParams, and passing that to a
           * LexicalizedParser constructor.  Note that despite this
           * method being an instance method, many flags are actually set as
           * static class variables.
           *
           * @param flags Arguments to the parser, for example,
           *              {"-outputFormat", "typedDependencies", "-maxLength", "70"}
           * @throws IllegalArgumentException If an unknown flag is passed in
           */
          public void setOptionFlags(String... flags) {
            op.setOptions(flags);
          }
    }
}
